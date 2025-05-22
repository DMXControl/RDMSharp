using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace RDMSharp
{
    public interface IRDMRemoteDevice: IRDMDevice
    {
        bool AllDataPulled { get; }
        bool Present { get; }
        DateTime LastSeen { get; }
        Task<bool> SetParameter(ERDM_Parameter parameter, object value = null);
    }
    public interface IRDMRemoteSubDevice : IRDMRemoteDevice
    {
    }
    public abstract class AbstractRemoteRDMDevice : AbstractRDMDevice , IRDMRemoteDevice
    {
        public sealed override bool IsGenerated => false;

        private readonly ConcurrentDictionary<byte, Sensor> sensors = new ConcurrentDictionary<byte, Sensor>();
        public sealed override IReadOnlyDictionary<byte, Sensor> Sensors { get { return sensors.AsReadOnly(); } }
        public sealed override IReadOnlyDictionary<ushort, Slot> Slots { get { return PersonalityModel.Slots; } }

        private RDMDeviceInfo deviceInfo;
        public override RDMDeviceInfo DeviceInfo { get { return deviceInfo; } }

        private RDMDeviceModel deviceModel;
        public RDMDeviceModel DeviceModel
        {
            get { return deviceModel; }
            private set
            {
                if (deviceModel == value)
                    return;
                deviceModel = value;
                OnPropertyChanged(nameof(DeviceModel));
            }
        }
        private RDMPersonalityModel personalityModel;
        public RDMPersonalityModel PersonalityModel
        {
            get { return personalityModel; }
            private set
            {
                if (personalityModel == value)
                    return;
                personalityModel = value;
                OnPropertyChanged(nameof(PersonalityModel));
            }
        }
        public sealed override IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get
            {
                if (DeviceModel == null)
                    return parameterValues.AsReadOnly();

                return DeviceModel.ParameterValues.Where(x => !parameterValues.ContainsKey(x.Key)).Concat(parameterValues).ToDictionary(k => k.Key, v => v.Value).AsReadOnly();
            }
        }

        private bool allDataPulled;
        public bool AllDataPulled
        {
            get
            {
                return allDataPulled;
            }
            private set
            {
                if (allDataPulled == value)
                    return;
                allDataPulled = value;
                OnPropertyChanged(nameof(AllDataPulled));
            }
        }

        private DateTime lastSeen;
        public DateTime LastSeen
        {
            get
            {
                return lastSeen;
            }
            private set
            {
                if (lastSeen == value)
                    return;
                lastSeen = value;
                OnPropertyChanged(nameof(LastSeen));
            }
        }

        private DateTime lastSendQueuedMessage;

        private bool present;
        public bool Present
        {
            get
            {
                return present;
            }
            internal set
            {
                if (present == value)
                    return;
                present = value;
                OnPropertyChanged(nameof(Present));
            }
        }


        public AbstractRemoteRDMDevice(UID uid) : base(uid)
        {
        }
        public AbstractRemoteRDMDevice(UID uid, SubDevice? subDevice = null) : base(uid, subDevice)
        {
            if (subDevice.HasValue && subDevice.Value.IsBroadcast)
                throw new NotSupportedException("A SubDevice can't be Broadcast.");
        }
        protected override async void initialize(RDMDeviceInfo deviceInfo = null)
        {
            base.initialize(deviceInfo);
            ParameterValueAdded += AbstractRDMDevice_ParameterValueAdded;
            ParameterValueChanged += AbstractRDMDevice_ParameterValueChanged;
            await requestDeviceInfo(deviceInfo);
        }

        private async void DeviceModel_Initialized(object sender, EventArgs e)
        {
            deviceModel.Initialized -= DeviceModel_Initialized;
            await collectParameters();
        }
        private async Task collectParameters()
        {
            await collectAllParametersOnRoot();
            await scanSubDevices();
            AllDataPulled = true;
            GlobalTimers.Instance.ParameterUpdateTimerElapsed += Instance_ParameterUpdateTimerElapsed;
        }

        private async Task scanSubDevices()
        {
            if (DeviceInfo.SubDeviceCount == 0)
                return;

            ushort foundSubDevices = 0;
            ushort currentID = 0;
            var dict = new Dictionary<SubDevice, RDMDeviceInfo>();
            while (foundSubDevices < DeviceInfo.SubDeviceCount)
            {
                currentID++;
                if (currentID > 512)
                    break;

                PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, new SubDevice(currentID), new ParameterBag(ERDM_Parameter.DEVICE_INFO));
                await runPeerToPeerProcess(ptpProcess);
                if (ptpProcess.ResponsePayloadObject.ParsedObject is RDMDeviceInfo _deviceInfo)
                {
                    foundSubDevices++;
                    var subDevice = createSubDevice(UID, ptpProcess.SubDevice);
                    this.SubDevices_Internal.Add(subDevice);
                    dict.Add(ptpProcess.SubDevice, _deviceInfo);
                }
            }
            foreach (AbstractRemoteRDMDevice sd in this.SubDevices_Internal)
            {
                if (sd.Subdevice.IsRoot)
                    continue;
                sd.asyncRDMRequestHelper = this.asyncRDMRequestHelper;
                sd.performInitialize(dict[sd.Subdevice]);
            }
        }

        protected abstract IRDMRemoteSubDevice createSubDevice(UID uid, SubDevice subDevice);

        #region Requests

        private async Task requestDeviceInfo(RDMDeviceInfo deviceInfo = null)
        {
            DataTreeBranch? rpl = null;
            if (deviceInfo == null)
            {
                PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, Subdevice, new ParameterBag(ERDM_Parameter.DEVICE_INFO));
                await runPeerToPeerProcess(ptpProcess);
                rpl= ptpProcess.ResponsePayloadObject;
                if (rpl.Value.ParsedObject is RDMDeviceInfo _deviceInfo)
                {
                    this.deviceInfo = _deviceInfo;
                    updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, ptpProcess.ResponsePayloadObject);
                    updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), ptpProcess.ResponsePayloadObject);
                    await getDeviceModelAndCollectAllParameters();
                }
            }
            else
            {
                rpl = DataTreeBranch.FromObject(deviceInfo, null, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEVICE_INFO);
                this.deviceInfo = deviceInfo;
                updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, rpl.Value);
                updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), rpl.Value);
                await getDeviceModelAndCollectAllParameters();
            }
        }
        private async Task requestParameters()
        {
            var parameters = this.DeviceModel?.SupportedNonBlueprintParameters;
            if (parameters == null)
                return;

            foreach (ERDM_Parameter parameter in parameters)
            {
                switch (parameter)
                {
                    case ERDM_Parameter.DEVICE_INFO:
                        continue;
                }
                await requestParameter(parameter);
            }
        }
        private async Task requestParameter(ERDM_Parameter parameter, object payload = null)
        {
            ParameterBag parameterBag = new ParameterBag(parameter, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define.GetRequest.HasValue)
            {
                if (define.GetRequest.Value.GetIsEmpty())
                    await requestGetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                else
                    await requestGetParameterWithPayload(parameterBag, define, UID, Subdevice, payload);
            }
        }

        private async Task updateParameters(bool queued = true)
        {
            if (queued && !deviceModel.KnownNotSupportedParameters.Contains(ERDM_Parameter.QUEUED_MESSAGE))
            {
                if (DateTime.UtcNow - lastSendQueuedMessage < TimeSpan.FromSeconds(4))
                    return;
                ParameterBag parameterBag = new ParameterBag(ERDM_Parameter.QUEUED_MESSAGE, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define.GetRequest.HasValue)
                {
                    byte mc = 0;
                    do
                    {
                        lastSendQueuedMessage = DateTime.UtcNow;
                        mc = await requestGetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                    }
                    while (mc != 0);
                }
                return;
            }
            else
            {
                while(ParameterUpdatedBag.TryPeek(out ParameterUpdatedBag bag))
                {
                     if (DateTime.UtcNow - bag.Timestamp < TimeSpan.FromSeconds(10))
                        return;

                    await requestParameter(bag.Parameter, bag.Index);

                    ParameterUpdatedBag.TryDequeue(out bag);
                }
            }
        }

        #endregion
        private async Task getDeviceModelAndCollectAllParameters()
        {
            if (deviceModel != null)
                return;
            deviceModel = RDMDeviceModel.getDeviceModel(UID, Subdevice, DeviceInfo, new Func<RDMMessage, Task>(SendRDMMessage));
            if (!deviceModel.IsInitialized)
            {
                deviceModel.Initialized += DeviceModel_Initialized;
                await deviceModel.Initialize();
            }
            else
                await collectParameters();
        }
        private async Task collectAllParametersOnRoot()
        {
            await requestParameters();
        }

        private async Task getPersonalityModelAndCollectAllParameters()
        {
            if (personalityModel != null)
                return;

            byte? personalityId = DeviceInfo.Dmx512CurrentPersonality.Value;
            if (parameterValues.TryGetValue(ERDM_Parameter.DMX_PERSONALITY_ID, out object value) && value is RDMDMXPersonality personality)
                personalityId = personality.CurrentPersonality;

            personalityModel = await DeviceModel.getPersonalityModel(this);
        }

        private async void AbstractRDMDevice_ParameterValueAdded(object sender, ParameterValueAddedEventArgs e)
        {
            if (!Constants.BLUEPRINT_MODEL_PARAMETERS.Contains(e.Parameter) && !Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS.Contains(e.Parameter))
                ParameterUpdatedBag.Enqueue(new ParameterUpdatedBag(e.Parameter, e.Index));
            switch (e.Parameter)
            {
                case ERDM_Parameter.DMX_PERSONALITY:
                    await getPersonalityModelAndCollectAllParameters();
                    break;
                case ERDM_Parameter.SENSOR_VALUE when e.Value is RDMSensorValue sensorValue:
                    var sensor = sensors.GetOrAdd(sensorValue.SensorId, (a) => new Sensor(a));
                    if (deviceModel.GetSensorDefinitions()?.FirstOrDefault(a => a.SensorId == sensorValue.SensorId) is RDMSensorDefinition sd)
                        sensor.UpdateDescription(sd);
                    sensor.UpdateValue(sensorValue);
                    break;
            }
        }
        private void AbstractRDMDevice_ParameterValueChanged(object sender, ParameterValueChangedEventArgs e)
        {
            if (!Constants.BLUEPRINT_MODEL_PARAMETERS.Contains(e.Parameter) && !Constants.BLUEPRINT_MODEL_PERSONALITY_PARAMETERS.Contains(e.Parameter))
            {
                if (ParameterUpdatedBag.Any(p => p.Parameter == e.Parameter && p.Index == e.Index))
                {
                    var tempQueue = new ConcurrentQueue<ParameterUpdatedBag>();
                    while (ParameterUpdatedBag.TryDequeue(out var item))
                        if (!(item.Parameter.Equals(e.Parameter) && Equals(item.Index, e.Index)))
                            tempQueue.Enqueue(item);


                    while (tempQueue.TryDequeue(out var item))
                        ParameterUpdatedBag.Enqueue(item);
                }
                ParameterUpdatedBag.Enqueue(new ParameterUpdatedBag(e.Parameter, e.Index));
            }

            switch (e.Parameter)
            {
                case ERDM_Parameter.SENSOR_VALUE when e.NewValue is RDMSensorValue sensorValue:
                    var sensor = sensors.GetOrAdd(sensorValue.SensorId, (a) => new Sensor(a));
                    sensor.UpdateValue(sensorValue);
                    break;
            }
        }

        public async Task<bool> SetParameter(ERDM_Parameter parameter, object value = null)
        {
            try
            {
                ParameterBag parameterBag = new ParameterBag(parameter, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define.SetRequest.HasValue)
                {
                    if (define.SetRequest.Value.GetIsEmpty())
                        await requestSetParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                    else
                        await requestSetParameterWithPayload(parameterBag, define, UID, Subdevice, value);
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(string.Empty, e);
            }
            return false;
        }
        private async Task<RequestResult> requestParameter(RDMMessage rdmMessage)
        {
            return await asyncRDMRequestHelper.RequestMessage(rdmMessage);
        }

        protected sealed override async Task OnReceiveRDMMessage(RDMMessage rdmMessage)
        {
            try
            {
                if (deviceModel != null)
                    deviceModel?.ReceiveRDMMessage(rdmMessage);
            }
            catch (Exception e)
            {
                await Task.CompletedTask;
                Logger?.LogError(e, string.Empty);
            }
            
            if (rdmMessage.SourceUID != UID || !rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            LastSeen = DateTime.UtcNow;

            if (asyncRDMRequestHelper?.ReceiveMessage(rdmMessage) ?? false)
                return;

            if ((rdmMessage.NackReason?.Length ?? 0) != 0)
                if (this.deviceModel?.handleNACKReason(rdmMessage) == false)
                    return;

            if (deviceModel?.IsInitialized == false)
                return;
        }
        public sealed override IReadOnlyDictionary<ERDM_Parameter, object> GetAllParameterValues()
        {
            if (this.DeviceModel != null)
                return this.DeviceModel.ParameterValues
                    .Concat(this.ParameterValues)
                    .ToLookup(x => x.Key, x => x.Value)
                    .ToDictionary(x => x.Key, g => g.First());
            else
                return base.GetAllParameterValues();
        }


        private async void Instance_ParameterUpdateTimerElapsed(object sender, EventArgs e)
        {
            await updateParameters();
        }

        public override string ToString()
        {
            return $"{base.ToString()} {this.DeviceModel}";
        }
        protected sealed override void OnDispose()
        {
            GlobalTimers.Instance.ParameterUpdateTimerElapsed -= Instance_ParameterUpdateTimerElapsed;
            try
            {
                onDispose();
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            ParameterValueAdded -= AbstractRDMDevice_ParameterValueAdded;
            ParameterValueChanged -= AbstractRDMDevice_ParameterValueChanged;
        }
        protected abstract void onDispose();

        
    }
}