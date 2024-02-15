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
    public abstract class AbstractRDMDevice : IRDMDevice
    {
        private static Random random = new Random();
        private static ILogger Logger = null;
        private static RDMParameterWrapperCatalogueManager pmManager => RDMParameterWrapperCatalogueManager.GetInstance();
        private static DeviceInfoParameterWrapper deviceInfoParameterWrapper => (DeviceInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEVICE_INFO);
        private static SensorValueParameterWrapper sensorValueParameterWrapper => (SensorValueParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SENSOR_VALUE);
        private static SlotInfoParameterWrapper slotInfoParameterWrapper => (SlotInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_INFO);
        private static SlotInfoParameterWrapper defaultSlotValueParameterWrapper => (SlotInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEFAULT_SLOT_VALUE);
        private static SlotDescriptionParameterWrapper slotDescriptionParameterWrapper => (SlotDescriptionParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_DESCRIPTION);
        private static StatusMessageParameterWrapper statusMessageParameterWrapper => (StatusMessageParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.STATUS_MESSAGES);


        
        private AsyncRDMRequestHelper asyncRDMRequestHelper;


        public event PropertyChangedEventHandler PropertyChanged;

        public RDMUID UID { get; private set; }
        public DateTime LastSeen { get; private set; }
        public bool Present { get; internal set; }

        public RDMDeviceInfo DeviceInfo { get; private set; }

        private RDMDeviceModel deviceModel;
        public RDMDeviceModel DeviceModel => deviceModel;

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
        public virtual bool IsGenerated { get; protected internal set; }
        public bool AllDataPulled { get; private set; }

        public AbstractRDMDevice(RDMUID uid)
        {
            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMRequestMessage);
            UID = uid;
            initialize();
        }
        private async void initialize()
        {
            if (!IsGenerated)
                await sendRDMRequestMessage(deviceInfoParameterWrapper.BuildGetRequestMessage());
        }

        protected void SetGeneratedParameterValue(ERDM_Parameter parameter, object value)
        {
            if (!IsGenerated)
                return;
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_INFO when value is RDMDeviceInfo deviceInfo:
                    DeviceInfo = deviceInfo;
                    goto default;

                default:
                    parameterValues.AddOrUpdate(parameter, value, (o, p) => value);
                    return;
            }
        }
        protected void SetGeneratedSensorValue(RDMSensorValue value)
        {
            if (!IsGenerated)
                return;
            sensorValues.AddOrUpdate(value.SensorId, value, (o, p) => value);
        }

        private async Task sendRDMRequestMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = UID;
            await SendRDMMessage(rdmMessage);
        }

        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);
        protected async Task ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            try
            {
                if (!IsGenerated)
                    if (deviceModel != null)
                        await deviceModel?.ReceiveRDMMessage(rdmMessage);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, string.Empty);
            }

            if ((rdmMessage.DestUID.IsBroadcast || rdmMessage.DestUID == UID) && !rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
            {
                await processRequestMessage(rdmMessage);
                return;
            }

            if (IsGenerated)
                return;

            if (rdmMessage.SourceUID != UID || !rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            LastSeen = DateTime.UtcNow;

            if (asyncRDMRequestHelper.ReceiveMethode(rdmMessage))
                return;

            if ((rdmMessage.NackReason?.Length ?? 0) != 0)
                if (this.deviceModel?.handleNACKReason(rdmMessage) == false)
                    return;

            if (deviceModel?.IsInitialized == false)
                return;

            await processResponseMessage(rdmMessage);
        }
        private async Task<RequestResult> requestParameter(RDMMessage rdmMessage)
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
            await UpdateParameterValues();
            await UpdateSensorValues();
            await UpdateSlotInfo();
            await UpdateDefaultSlotValue();
            await UpdateSlotDescriptions();
            AllDataPulled = true;
        }
        private async Task processRequestMessage(RDMMessage rdmMessage)
        {
            //await Task.Delay(200);
            var pm = pmManager.GetRDMParameterWrapperByID(rdmMessage.Parameter);
            object responseValue = null;
            parameterValues.TryGetValue(rdmMessage.Parameter, out responseValue);
            RDMMessage? response = null;
            if (rdmMessage.Command == ERDM_Command.GET_COMMAND)
            {
                switch (pm)
                {
                    case DeviceInfoParameterWrapper _deviceInfoParameterWrapper:
                        response = _deviceInfoParameterWrapper.BuildGetResponseMessage(DeviceInfo);
                        break;

                    case SupportedParametersParameterWrapper _supportedParametersParameterWrapper:
                        List<ERDM_Parameter> sp = new List<ERDM_Parameter>();
                        sp.Add(ERDM_Parameter.DEVICE_INFO);
                        sp.AddRange(parameterValues.Keys);
                        response = _supportedParametersParameterWrapper.BuildGetResponseMessage(sp.ToArray());
                        break;
                    case QueuedMessageParameterWrapper _queuedMessageParameterWrapper:
                        response = statusMessageParameterWrapper.BuildGetResponseMessage([]);
                        break;


                    case IRDMGetParameterWrapperResponse<string> getParameterWrapperResponseString
                    when responseValue is string _string:
                        response = getParameterWrapperResponseString.BuildGetResponseMessage(_string);
                        break;
                    case IRDMGetParameterWrapperResponse<bool> getParameterWrapperResponseBool
                    when responseValue is bool _bool:
                        response = getParameterWrapperResponseBool.BuildGetResponseMessage(_bool);
                        break;
                    case IRDMGetParameterWrapperResponse<byte> getParameterWrapperResponseByte
                    when responseValue is byte _byte:
                        response = getParameterWrapperResponseByte.BuildGetResponseMessage(_byte);
                        break;
                    case IRDMGetParameterWrapperResponse<sbyte> getParameterWrapperResponseSByte
                    when responseValue is sbyte _sbyte:
                        response = getParameterWrapperResponseSByte.BuildGetResponseMessage(_sbyte);
                        break;
                    case IRDMGetParameterWrapperResponse<short> getParameterWrapperResponseShort
                    when responseValue is short _short:
                        response = getParameterWrapperResponseShort.BuildGetResponseMessage(_short);
                        break;
                    case IRDMGetParameterWrapperResponse<ushort> getParameterWrapperResponseUShort
                    when responseValue is ushort _ushort:
                        response = getParameterWrapperResponseUShort.BuildGetResponseMessage(_ushort);
                        break;
                    case IRDMGetParameterWrapperResponse<int> getParameterWrapperResponseInt
                    when responseValue is int _int:
                        response = getParameterWrapperResponseInt.BuildGetResponseMessage(_int);
                        break;
                    case IRDMGetParameterWrapperResponse<uint> getParameterWrapperResponseUInt
                    when responseValue is uint _uint:
                        response = getParameterWrapperResponseUInt.BuildGetResponseMessage(_uint);
                        break;
                    case IRDMGetParameterWrapperResponse<long> getParameterWrapperResponseLong
                    when responseValue is long _long:
                        response = getParameterWrapperResponseLong.BuildGetResponseMessage(_long);
                        break;
                    case IRDMGetParameterWrapperResponse<ulong> getParameterWrapperResponseULong
                    when responseValue is ulong _ulong:
                        response = getParameterWrapperResponseULong.BuildGetResponseMessage(_ulong);
                        break;
                    case IRDMGetParameterWrapperResponse<double> getParameterWrapperResponseDouble
                    when responseValue is double _double:
                        response = getParameterWrapperResponseDouble.BuildGetResponseMessage(_double);
                        break;
                    case IRDMGetParameterWrapperResponse<float> getParameterWrapperResponseFloat
                    when responseValue is float _float:
                        response = getParameterWrapperResponseFloat.BuildGetResponseMessage(_float);
                        break;
                    case DMX512StartingAddressParameterWrapper dmx512StartingAddressParameterWrapper
                        when responseValue is ushort _ushort:
                        response = dmx512StartingAddressParameterWrapper.BuildGetResponseMessage(_ushort);
                        break;
                }
            }
            else if (rdmMessage.Command == ERDM_Command.SET_COMMAND)
            {
                //Handle set Requerst
                switch (pm)
                {
                    case IRDMSetParameterWrapperRequestContravariance<bool> setParameterWrapperRequestContravarianceBool
                    when setParameterWrapperRequestContravarianceBool.SetRequestParameterDataToObject(rdmMessage.ParameterData) is bool _bool:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _bool, (x, y) => _bool);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<string> setParameterWrapperRequestContravarianceString
                    when setParameterWrapperRequestContravarianceString.SetRequestParameterDataToObject(rdmMessage.ParameterData) is string _string:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _string, (x, y) => _string);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<byte> setParameterWrapperRequestContravarianceByte
                    when setParameterWrapperRequestContravarianceByte.SetRequestParameterDataToObject(rdmMessage.ParameterData) is byte _byte:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _byte, (x, y) => _byte);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<sbyte> setParameterWrapperRequestContravarianceSByte
                    when setParameterWrapperRequestContravarianceSByte.SetRequestParameterDataToObject(rdmMessage.ParameterData) is sbyte _sbyte:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _sbyte, (x, y) => _sbyte);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<short> setParameterWrapperRequestContravarianceShort
                    when setParameterWrapperRequestContravarianceShort.SetRequestParameterDataToObject(rdmMessage.ParameterData) is short _short:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _short, (x, y) => _short);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<ushort> setParameterWrapperRequestContravarianceUShort
                    when setParameterWrapperRequestContravarianceUShort.SetRequestParameterDataToObject(rdmMessage.ParameterData) is ushort _ushort:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _ushort, (x, y) => _ushort);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<int> setParameterWrapperRequestContravarianceInt
                    when setParameterWrapperRequestContravarianceInt.SetRequestParameterDataToObject(rdmMessage.ParameterData) is int _int:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _int, (x, y) => _int);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<uint> setParameterWrapperRequestContravarianceUInt
                    when setParameterWrapperRequestContravarianceUInt.SetRequestParameterDataToObject(rdmMessage.ParameterData) is uint _uint:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _uint, (x, y) => _uint);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<long> setParameterWrapperRequestContravarianceLong
                    when setParameterWrapperRequestContravarianceLong.SetRequestParameterDataToObject(rdmMessage.ParameterData) is long _long:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _long, (x, y) => _long);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<ulong> setParameterWrapperRequestContravarianceULong
                    when setParameterWrapperRequestContravarianceULong.SetRequestParameterDataToObject(rdmMessage.ParameterData) is ulong _ulong:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _ulong, (x, y) => _ulong);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<double> setParameterWrapperRequestContravarianceDouble
                    when setParameterWrapperRequestContravarianceDouble.SetRequestParameterDataToObject(rdmMessage.ParameterData) is double _double:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _double, (x, y) => _double);
                        break;

                    case IRDMSetParameterWrapperRequestContravariance<float> setParameterWrapperRequestContravarianceFloat
                    when setParameterWrapperRequestContravarianceFloat.SetRequestParameterDataToObject(rdmMessage.ParameterData) is float _float:
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, _float, (x, y) => _float);
                        break;
                }
                //Do set Response
                object val = null;
                switch (pm)
                {
                    case IRDMSetParameterWrapperWithEmptySetResponse setParameterWrapperResponseContravarianceEmpty:
                        response = setParameterWrapperResponseContravarianceEmpty.BuildSetResponseMessage();
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<bool> setParameterWrapperResponseContravarianceBool:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is bool _bool)
                            response = setParameterWrapperResponseContravarianceBool.BuildSetResponseMessage(_bool);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<string> setParameterWrapperResponseContravarianceString:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is string _string)
                            response = setParameterWrapperResponseContravarianceString.BuildSetResponseMessage(_string);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<byte> setParameterWrapperResponseContravarianceByte:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is byte _byte)
                            response = setParameterWrapperResponseContravarianceByte.BuildSetResponseMessage(_byte);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<sbyte> setParameterWrapperResponseContravarianceSByte:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is sbyte _sbyte)
                            response = setParameterWrapperResponseContravarianceSByte.BuildSetResponseMessage(_sbyte);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<short> setParameterWrapperResponseContravarianceShort:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is short _short)
                            response = setParameterWrapperResponseContravarianceShort.BuildSetResponseMessage(_short);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<ushort> setParameterWrapperResponseContravarianceUShort:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is ushort _ushort)
                            response = setParameterWrapperResponseContravarianceUShort.BuildSetResponseMessage(_ushort);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<int> setParameterWrapperResponseContravarianceInt:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is int _int)
                            response = setParameterWrapperResponseContravarianceInt.BuildSetResponseMessage(_int);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<uint> setParameterWrapperResponseContravarianceUInt:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is uint _uint)
                            response = setParameterWrapperResponseContravarianceUInt.BuildSetResponseMessage(_uint);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<long> setParameterWrapperResponseContravarianceLong:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is long _long)
                            response = setParameterWrapperResponseContravarianceLong.BuildSetResponseMessage(_long);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<ulong> setParameterWrapperResponseContravarianceULong:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is ulong _ulong)
                            response = setParameterWrapperResponseContravarianceULong.BuildSetResponseMessage(_ulong);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<double> setParameterWrapperResponseContravarianceDouble:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is double _double)
                            response = setParameterWrapperResponseContravarianceDouble.BuildSetResponseMessage(_double);
                        break;

                    case IRDMSetParameterWrapperSetResponseContravariance<float> setParameterWrapperResponseContravarianceFloat:
                        parameterValues.TryGetValue(rdmMessage.Parameter, out val);
                        if (val is float _float)
                            response = setParameterWrapperResponseContravarianceFloat.BuildSetResponseMessage(_float);
                        break;
                }
            }
            if (rdmMessage.DestUID.IsBroadcast) // no Response on Broadcast
                return;
            if (response == null)
                response = new RDMMessage(ERDM_NackReason.UNKNOWN_PID) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };

            response.TransactionCounter = rdmMessage.TransactionCounter;
            response.SourceUID = rdmMessage.DestUID;
            response.DestUID = rdmMessage.SourceUID;
            //await Task.Delay(33);
            await SendRDMMessage(response);
        }

        private async Task processResponseMessage(RequestResult result)
        {
            if (IsGenerated)
                return;

            if (result.Success)
                await processResponseMessage(result.Response);
            else if (result.Cancel)
                return;
            else
            {
                await Task.Delay(TimeSpan.FromTicks(random.Next(4500, 5500)));
                await processResponseMessage(await requestParameter(result.Request));
            }
        }
        private async Task processResponseMessage(RDMMessage rdmMessage)
        {
            if (IsGenerated)
                return;

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
                Logger?.LogError(string.Empty, ex);
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

                    deviceModel = RDMDeviceModel.getDeviceModel(UID, deviceInfo, new Func<RDMMessage,Task>(SendRDMMessage));
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
                            Logger?.LogError($"The response does not contain the expected data {typeof(RDMSlotDescription)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger?.LogTrace($"No response received");
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

                        Logger?.LogError($"The response does not contain the expected data {typeof(RDMSlotInfo[])}!{Environment.NewLine}{rdmMessage}");
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
                        Logger?.LogError($"The response does not contain the expected data {typeof(RDMDefaultSlotValue[])}!{Environment.NewLine}{rdmMessage}");
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
                            Logger?.LogError($"The response does not contain the expected data {typeof(RDMSensorValue)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger?.LogError($"No response received");
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
            if (IsGenerated)
                return;

            if (this.DeviceModel == null)
                return;

            try
            {
                foreach (ERDM_Parameter parameter in this.DeviceModel.SupportedNonBlueprintParameters)
                    await this.UpdateParameterValue(parameter);
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to get UpdateParameterValues for UID: {this.UID}", e);
            }
        }
        public async Task UpdateParameterValue(ERDM_Parameter parameterId)
        {
            if (IsGenerated)
                return;

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
                Logger?.LogDebug("Not Implemented Parameter");
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
                        tasks.Add(processResponseMessage(await requestParameter(@emptyGetRequest.BuildGetRequestMessage())));
                        break;
                    case IRDMGetParameterWrapperRequest<byte> @byteGetRequest:
                        foreach (ERDM_Parameter para in @byteGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @byteGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processResponseMessage(await requestParameter(@byteGetRequest.BuildGetRequestMessage(r))));

                        break;
                    case IRDMGetParameterWrapperRequest<ushort> @ushortGetRequest:
                        foreach (ERDM_Parameter para in @ushortGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @ushortGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processResponseMessage(await requestParameter(@ushortGetRequest.BuildGetRequestMessage(r))));

                        break;
                    case IRDMGetParameterWrapperRequest<uint> @uintGetRequest:
                        foreach (ERDM_Parameter para in @uintGetRequest.DescriptiveParameters)
                        {
                            this.DeviceModel.ParameterValues.TryGetValue(para, out val);
                            if (val != null)
                                break;
                        }
                        foreach (var r in @uintGetRequest.GetRequestRange(val).ToEnumerator())
                            tasks.Add(processResponseMessage(await requestParameter(@uintGetRequest.BuildGetRequestMessage(r))));

                        break;

                    case StatusMessageParameterWrapper statusMessageParameter:
                        tasks.Add(processResponseMessage(await requestParameter(statusMessageParameter.BuildGetRequestMessage(ERDM_Status.ADVISORY))));
                        break;
                    default:
                        Logger?.LogDebug($"No Wrapper for Parameter: {parameterId} for UID: {this.UID}");
                        break;
                }
                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to update ParameterValue of Parameter: {parameterId} for UID: {this.UID}", e);
            }
            this.pendingParametersUpdateRequest.Remove(parameterId);
        }
        public async Task UpdateSensorValues()
        {
            if (IsGenerated)
                return;

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
                Logger?.LogError($"Not able to update SensorValues for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSensorValue(byte sensorId)
        {
            if (IsGenerated)
                return;

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
                await processResponseMessage(await requestParameter(sensorValueParameterWrapper.BuildGetRequestMessage(sensorId)));
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to update SensorValue of Sensor: {sensorId} for UID: {this.UID}", e);
            }
            this.pendingSensorValuesUpdateRequest.Remove(sensorId);
        }
        public async Task UpdateSlotInfo()
        {
            if (IsGenerated)
                return;

            if (this.DeviceInfo == null || this.slots == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_INFO) != true)
                return;

            try
            {
                RequestResult? result = null;
                do
                {
                    result = await requestParameter(slotInfoParameterWrapper.BuildGetRequestMessage());
                    if (result.Value.Success)
                        await processResponseMessage(result.Value.Response);
                    else if (result.Value.Cancel)
                        return;
                    else
                        await Task.Delay(TimeSpan.FromTicks(random.Next(2500, 3500)));
                }
                while (result?.Response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW || result?.Response == null);
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to update SlotInfo for UID: {this.UID}", e);
            }
        }
        public async Task UpdateDefaultSlotValue()
        {
            if (IsGenerated)
                return;

            if (this.DeviceInfo == null || this.slots == null)
                return;

            if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.DEFAULT_SLOT_VALUE) != true)
                return;

            try
            {
                RequestResult? result = null;
                do
                {
                    result = await requestParameter(defaultSlotValueParameterWrapper.BuildGetRequestMessage());
                    if (result.Value.Success)
                        await processResponseMessage(result.Value.Response);
                    else if (result.Value.Cancel)
                        return;
                    else
                        await Task.Delay(TimeSpan.FromTicks(random.Next(2500, 3500)));
                }
                while (result?.Response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW || result?.Response == null);
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to update DefaultSlotValue for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSlotDescriptions()
        {
            if (IsGenerated)
                return;

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
                Logger?.LogError($"Not able to update SlotDescriptions for UID: {this.UID}", e);
            }
        }
        public async Task UpdateSlotDescription(ushort slotId)
        {
            if (IsGenerated)
                return;

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
                await processResponseMessage(await requestParameter(slotDescriptionParameterWrapper.BuildGetRequestMessage(slotId)));
            }
            catch (Exception e)
            {
                Logger?.LogError($"Not able to update SlotDescription of Slot: {slotId} for UID: {this.UID}", e);
            }
            this.pendingSlotDescriptionsUpdateRequest.Remove(slotId);
        }

        public IReadOnlyDictionary<ERDM_Parameter, object> GetAllParameterValues() 
        {
            if (this.DeviceModel != null)
                return this.DeviceModel.ParameterValues
                    .Concat(this.ParameterValues)
                    .ToLookup(x => x.Key, x => x.Value)
                    .ToDictionary(x => x.Key, g => g.First());
            else
                return this.ParameterValues;
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
    public abstract class AbstractGeneratedRDMDevice : AbstractRDMDevice
    {
        public sealed override bool IsGenerated => true;
        public AbstractGeneratedRDMDevice(RDMUID uid) : base(uid)
        {
        }
    }
}