using Microsoft.Extensions.Logging;
using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractRDMDevice<T_RDMDeviceModel> : IRDMDevice where T_RDMDeviceModel : AbstractRDMDeviceModel
    {
        private static ILogger Logger = null;
        private static RDMParameterWrapperCatalogueManager pmManager => RDMParameterWrapperCatalogueManager.GetInstance();
        private static DeviceInfoParameterWrapper deviceInfoParameterWrapper => (DeviceInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEVICE_INFO);
        private static SensorValueParameterWrapper sensorValueParameterWrapper => (SensorValueParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SENSOR_VALUE);
        private static SlotInfoParameterWrapper slotInfoParameterWrapper => (SlotInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_INFO);
        private static SlotInfoParameterWrapper defaultSlotValueParameterWrapper => (SlotInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEFAULT_SLOT_VALUE);
        private static SlotDescriptionParameterWrapper slotDescriptionParameterWrapper => (SlotDescriptionParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_DESCRIPTION);


        private static List<T_RDMDeviceModel> knownDeviceModels = new List<T_RDMDeviceModel>();
        public static IReadOnlyCollection<T_RDMDeviceModel> KnownDeviceModels => knownDeviceModels.AsReadOnly();
        private static T_RDMDeviceModel getDeviceModel(RDMUID uid, RDMDeviceInfo deviceInfo)
        {
            var kdm = knownDeviceModels.FirstOrDefault(dm => dm.IsModelOf(uid, deviceInfo));
            if (kdm == null)
                kdm = (T_RDMDeviceModel)Activator.CreateInstance(typeof(T_RDMDeviceModel), uid, deviceInfo);

            return kdm;

        }
        private AsyncRDMRequestHelper asyncRDMRequestHelper;


        public event PropertyChangedEventHandler PropertyChanged;

        public RDMUID UID { get; private set; }
        public DateTime LastSeen { get; private set; }
        public bool Present { get; internal set; }

        public RDMDeviceInfo DeviceInfo { get; private set; }

        private T_RDMDeviceModel deviceModel;
        public IRDMDeviceModel DeviceModel => deviceModel;

        private ConcurrentDictionary<ERDM_Parameter, object> parameterValues = new ConcurrentDictionary<ERDM_Parameter, object>();
        public IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues => parameterValues.AsReadOnly();
        private HashSet<ERDM_Parameter> pendingParametersUpdateRequest = new HashSet<ERDM_Parameter>();

        private ConcurrentDictionary<byte, RDMSensorValue> sensorValues = new ConcurrentDictionary<byte, RDMSensorValue>();
        public IReadOnlyDictionary<byte, RDMSensorValue> SensorValues => sensorValues.AsReadOnly();
        private HashSet<byte> pendingSensorValuesUpdateRequest = new HashSet<byte>();

        private ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();
        public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();
        private HashSet<ushort> pendingSlotDescriptionsUpdateRequest = new HashSet<ushort>();

        public bool IsDisposing { get; private set; }

        public bool IsDisposed { get; private set; }

        public AbstractRDMDevice(RDMUID uid)
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMMessage);
            UID = uid;
            initialize();
        }
        private void initialize()
        {
            sendRDMMessage(deviceInfoParameterWrapper.BuildGetRequestMessage());
        }

        private void sendRDMMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = UID;
            _ = SendRDMMessage(rdmMessage);
        }

        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);
        protected async Task ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage.SourceUID != UID)
                return;

            if (!rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            LastSeen = DateTime.UtcNow;

            if (asyncRDMRequestHelper.ReceiveMethode(rdmMessage))
                return;

            if ((rdmMessage.NackReason?.Length ?? 0) != 0)
                if (this.deviceModel?.handleNACKReason(rdmMessage) == false)
                    return;

            if (deviceModel?.IsInitialized == false)
                return;

            await processMessage(rdmMessage);
        }
        private async Task<RDMMessage> requestParameter(RDMMessage rdmMessage)
        {
            return await asyncRDMRequestHelper.RequestParameter(rdmMessage);
        }
        private async void DeviceModel_Initialized(object sender, EventArgs e)
        {
            deviceModel.Initialized -= DeviceModel_Initialized;
            await collectAllParameters();
        }
        private async Task collectAllParameters()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(UpdateParameterValues());
            tasks.Add(UpdateSensorValues());
            tasks.Add(UpdateSlotInfo());
            tasks.Add(UpdateDefaultSlotValue());
            tasks.Add(UpdateSlotDescriptions());
            await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(30)));
        }
        private async Task processMessage(RDMMessage rdmMessage)
        {
            if (rdmMessage.Parameter != ERDM_Parameter.DEVICE_INFO && (this.DeviceModel?.SupportedBlueprintParameters.Contains(rdmMessage.Parameter) ?? false))
                return;

            if (rdmMessage.NackReason != null)
                if (rdmMessage.NackReason.Length != 0)
                    return;

            var pm = pmManager.GetRDMParameterWrapperByID(rdmMessage.Parameter);
            object value = null;
            try
            {
                value = rdmMessage.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Empty, ex);
            }
            if (value == null)
                return;
            switch (pm)
            {
                case DeviceInfoParameterWrapper _deviceInfoParameterWrapper:
                    if (!(rdmMessage.Value is RDMDeviceInfo deviceInfo))
                        break;

                    DeviceInfo = deviceInfo;

                    if (deviceModel != null)
                        break;

                    deviceModel = getDeviceModel(UID, deviceInfo);
                    if (!deviceModel.IsInitialized)
                    {
                        deviceModel.Initialized += DeviceModel_Initialized;
                        await deviceModel.Initialize();
                    }
                    else
                        await collectAllParameters();

                    break;

                case SlotDescriptionParameterWrapper _slotDescriptionParameterWrapper:
                    if (!(value is RDMSlotDescription description))
                    {
                        if (value != null)
                            Logger.LogError($"The response does not contain the expected data {typeof(RDMSlotDescription)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger.LogTrace($"No response received");
                        return;
                    }
                    Slot slot;
                    if (!slots.TryGetValue(description.SlotId, out slot))
                    {
                        slot = new Slot(description.SlotId);
                        slots.TryAdd(slot.SlotId, slot);
                    }
                    slot.Description = description.Description;
                    this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(this.Slots)));
                    break;

                case SlotInfoParameterWrapper _slotInfoParameterWrapper:
                    if (!(value is RDMSlotInfo[] slotInfos))
                    {
                        if (rdmMessage.NackReason.Contains(ERDM_NackReason.ACTION_NOT_SUPPORTED))
                            this.slots = null; //Set to null, to Deactivate this UpdateSlotInfo

                        Logger.LogError($"The response does not contain the expected data {typeof(RDMSlotInfo[])}!{Environment.NewLine}{rdmMessage}");
                        return;
                    }

                    foreach (RDMSlotInfo info in slotInfos)
                    {
                        Slot slot1;
                        if (!slots.TryGetValue(info.SlotOffset, out slot1))
                        {
                            slot1 = new Slot(info.SlotOffset);
                            slots.TryAdd(info.SlotOffset, slot1);
                        }
                        slot1.Type = info.SlotType;
                        slot1.Category = info.SlotLabelId;
                    }
                    this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(this.Slots)));
                    break;

                case DefaultSlotValueParameterWrapper _defaultSlotValueParameterWrapper:
                    if (!(value is RDMDefaultSlotValue[] defaultSlotValues))
                    {
                        Logger.LogError($"The response does not contain the expected data {typeof(RDMDefaultSlotValue[])}!{Environment.NewLine}{rdmMessage}");
                        return;
                    }

                    foreach (RDMDefaultSlotValue info in defaultSlotValues)
                    {
                        Slot slot1;
                        if (!slots.TryGetValue(info.SlotOffset, out slot1))
                        {
                            slot1 = new Slot(info.SlotOffset);
                            slots.TryAdd(info.SlotOffset, slot1);
                        }
                        slot1.DefaultValue = info.DefaultSlotValue;
                    }
                    this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(this.Slots)));
                    break;
                case SensorDefinitionParameterWrapper _sensorDefinitionParameterWrapper:
                    if (!(value is RDMSensorValue sensorValue))
                    {
                        if (value != null)
                            Logger.LogError($"The response does not contain the expected data {typeof(RDMSensorValue)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger.LogError($"No response received");
                        return;
                    }
                    sensorValues.AddOrUpdate(sensorValue.SensorId, sensorValue, (x, y) => sensorValue);
                    this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(nameof(this.SensorValues)));
                    break;

                case IRDMGetParameterWrapperWithEmptyGetRequest @emptyGetRequest:
                    parameterValues.AddOrUpdate(rdmMessage.Parameter, value, (x, y) => value);
                    break;

                case IRDMGetParameterWrapperRequest<byte> @emptyGetRequest:
                    parameterValues.AddOrUpdate(rdmMessage.Parameter, value, (x, y) => value);
                    break;
                case IRDMGetParameterWrapperRequest<ushort> @emptyGetRequest:
                    parameterValues.AddOrUpdate(rdmMessage.Parameter, value, (x, y) => value);
                    break;
                case IRDMGetParameterWrapperRequest<uint> @emptyGetRequest:
                    parameterValues.AddOrUpdate(rdmMessage.Parameter, value, (x, y) => value);
                    break;

                default:
                    break;

            }
        }

        public async Task UpdateParameterValues()
        {
            if (this.DeviceModel == null)
                return;

            try
            {
                List<Task> tasks = new List<Task>();
                foreach (ERDM_Parameter parameter in this.DeviceModel.SupportedNonBlueprintParameters)
                    tasks.Add(this.UpdateParameterValue(parameter));

                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(20)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to get UpdateParameterValues for UID: {this.UID}", e);
            }
        }
        public async Task UpdateParameterValue(ERDM_Parameter parameterId)
        {
            if (this.DeviceModel == null)
                return;
            if (this.pendingParametersUpdateRequest.Contains(parameterId))
                return;

            switch (parameterId)
            {
                case ERDM_Parameter.SENSOR_DEFINITION:
                case ERDM_Parameter.SENSOR_VALUE:
                case ERDM_Parameter.SLOT_INFO:
                case ERDM_Parameter.SLOT_DESCRIPTION:
                case ERDM_Parameter.DEFAULT_SLOT_VALUE:
                case ERDM_Parameter.QUEUED_MESSAGE:
                    return;
            }

            var pm = pmManager.GetRDMParameterWrapperByID(parameterId);
            if (pm == null && Enum.IsDefined(typeof(ERDM_Parameter), parameterId))
                return;

            if(pm==null)
                pm = deviceModel.GetRDMParameterWrapperByID((ushort)parameterId);
            if (pm == null)
            { 
                Logger.LogDebug("Not Implemented Parameter");
                return;
            }

            if (!pm.CommandClass.HasFlag(ERDM_CommandClass.GET))
                return;

            this.pendingParametersUpdateRequest.Add(parameterId);
            try
            {
                List<Task> tasks = new List<Task>();
                object val = null;
                switch (pm)
                {
                    case IRDMGetParameterWrapperWithEmptyGetRequest @emptyGetRequest:
                        tasks.Add(processMessage(await requestParameter(@emptyGetRequest.BuildGetRequestMessage())));
                        break;
                    case IRDMGetParameterWrapperRequest<byte> @byteGetRequest:
                        foreach (ERDM_Parameter para in @byteGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @byteGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processMessage(await requestParameter(@byteGetRequest.BuildGetRequestMessage(r))));

                        break;
                    case IRDMGetParameterWrapperRequest<ushort> @ushortGetRequest:
                        foreach (ERDM_Parameter para in @ushortGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @ushortGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processMessage(await requestParameter(@ushortGetRequest.BuildGetRequestMessage(r))));

                        break;
                    case IRDMGetParameterWrapperRequest<uint> @uintGetRequest:
                        foreach (ERDM_Parameter para in @uintGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @uintGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processMessage(await requestParameter(@uintGetRequest.BuildGetRequestMessage(r))));

                        break;

                    case StatusMessageParameterWrapper statusMessageParameter:
                        tasks.Add(processMessage(await requestParameter(statusMessageParameter.BuildGetRequestMessage(ERDM_Status.ADVISORY))));
                        break;
                    default:
                        break;
                }
                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update ParameterValue of Parameter: {parameterId} for UID: {this.UID}", e);
            }
            this.pendingParametersUpdateRequest.Remove(parameterId);
        }
        public async Task UpdateSensorValues()
        {
            if (this.DeviceInfo == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SENSOR_DEFINITION) != true)
                return;

            if (this.DeviceModel == null)
                return;

            if (this.DeviceInfo.SensorCount == 0)
                return;

            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var sd in this.DeviceModel.GetSensorDefinitions())
                    tasks.Add(this.UpdateSensorValue(sd.SensorId));

                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update SensorValues for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSensorValue(byte sensorId)
        {
            if (this.DeviceInfo == null)
                return;
            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SENSOR_VALUE) != true)
                return;
            if (this.pendingSensorValuesUpdateRequest.Contains(sensorId))
                return;

            if (this.DeviceInfo.SensorCount == 0)
                return;

            try
            {
                this.pendingSensorValuesUpdateRequest.Add(sensorId);
                await processMessage(await requestParameter(sensorValueParameterWrapper.BuildGetRequestMessage(sensorId)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update SensorValue of Sensor: {sensorId} for UID: {this.UID}", e);
            }
            this.pendingSensorValuesUpdateRequest.Remove(sensorId);
        }
        public async Task UpdateSlotInfo()
        {
            if (this.DeviceInfo == null || this.slots == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_INFO) != true)
                return;

            try
            {
                RDMMessage response = null;
                do
                {
                    response = await requestParameter(slotInfoParameterWrapper.BuildGetRequestMessage());
                    await processMessage(response);
                }
                while (response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW);
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update SlotInfo for UID: {this.UID}", e);
            }
        }
        public async Task UpdateDefaultSlotValue()
        {
            if (this.DeviceInfo == null || this.slots == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.DEFAULT_SLOT_VALUE) != true)
                return;

            try
            {
                RDMMessage response = null;
                do
                {
                    response = await requestParameter(defaultSlotValueParameterWrapper.BuildGetRequestMessage());
                    await processMessage(response);
                }
                while (response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW);
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update DefaultSlotValue for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSlotDescriptions()
        {
            if (this.DeviceInfo == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_DESCRIPTION) != true)
                return;

            if (this.Slots.Count == 0)
                return;

            try
            {
                List<Task> tasks = new List<Task>();
                foreach (var slot in this.slots)
                    tasks.Add(this.UpdateSlotDescription(slot.Key));

                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update SlotDescriptions for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSlotDescription(ushort slotId)
        {
            if (this.DeviceInfo == null)
                return;
            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_DESCRIPTION) != true)
                return;

            if (this.pendingSlotDescriptionsUpdateRequest.Contains(slotId))
                return;

            if (this.Slots.Count == 0)
                return;

            try
            {
                this.pendingSlotDescriptionsUpdateRequest.Add(slotId);
                await processMessage(await requestParameter(slotDescriptionParameterWrapper.BuildGetRequestMessage(slotId)));
            }
            catch (Exception e)
            {
                Logger.LogError($"Not able to update SlotDescription of Slot: {slotId} for UID: {this.UID}", e);
            }
            this.pendingSlotDescriptionsUpdateRequest.Remove(slotId);
        }

        public void Dispose()
        {
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;
            try
            {
                OnDispose();
            }
            catch { }
            finally
            {
                IsDisposed = true;
                IsDisposing = false;
            }
        }
        protected virtual void OnDispose()
        {

        }

        public override string ToString()
        {
            return $"[{UID}] {this.DeviceModel}";
        }
    }
}