using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp
{
    public abstract class AbstractRDMDevice : AbstractRDMCache, IRDMDevice
    {
        private static readonly Random random = new Random();
        private protected static readonly ILogger Logger = null;
        private protected static RDMParameterWrapperCatalogueManager pmManager => RDMParameterWrapperCatalogueManager.GetInstance();
        private protected static SensorValueParameterWrapper sensorValueParameterWrapper => (SensorValueParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SENSOR_VALUE);
        private protected static SlotInfoParameterWrapper slotInfoParameterWrapper => (SlotInfoParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_INFO);
        private protected static DefaultSlotValueParameterWrapper defaultSlotValueParameterWrapper => (DefaultSlotValueParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.DEFAULT_SLOT_VALUE);
        private protected static SlotDescriptionParameterWrapper slotDescriptionParameterWrapper => (SlotDescriptionParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.SLOT_DESCRIPTION);
        private protected static StatusMessageParameterWrapper statusMessageParameterWrapper => (StatusMessageParameterWrapper)pmManager.GetRDMParameterWrapperByID(ERDM_Parameter.STATUS_MESSAGES);

        public event PropertyChangedEventHandler PropertyChanged;

        public readonly UID uid;
        public readonly SubDevice subdevice;
        public UID UID => uid;
        public SubDevice Subdevice => subdevice;
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

        private bool discoveryMuted;
        public bool DiscoveryMuted
        {
            get
            {
                return discoveryMuted;
            }
            private set
            {
                if (discoveryMuted == value)
                    return;
                discoveryMuted = value;
                OnPropertyChanged(nameof(DiscoveryMuted));
            }
        }

        private List<IRDMDevice> subDevices;
        public IReadOnlyCollection<IRDMDevice> SubDevices { get { return subDevices.AsReadOnly(); } }

        public virtual RDMDeviceInfo DeviceInfo { get; private protected set; }

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

        public sealed override IReadOnlyDictionary<ERDM_Parameter, object> ParameterValues
        {
            get
            {
                if (DeviceModel == null)
                    return parameterValues.AsReadOnly();

                return DeviceModel.ParameterValues.Where(x => !parameterValues.ContainsKey(x.Key)).Concat(parameterValues).ToDictionary(k => k.Key, v => v.Value).AsReadOnly();
            }
        }
        private readonly HashSet<ERDM_Parameter> pendingParametersUpdateRequest = new HashSet<ERDM_Parameter>();

        private readonly ConcurrentDictionary<byte, Sensor> sensors = new ConcurrentDictionary<byte, Sensor>();
        public IReadOnlyDictionary<byte, Sensor> Sensors => sensors.AsReadOnly();
        private readonly HashSet<byte> pendingSensorValuesUpdateRequest = new HashSet<byte>();

        private ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();
        public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();
        private readonly HashSet<ushort> pendingSlotDescriptionsUpdateRequest = new HashSet<ushort>();

        public new bool IsDisposing { get; private set; }
        public new bool IsDisposed { get; private set; }
        public virtual bool IsGenerated { get; private protected set; }
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

        public AbstractRDMDevice(UID uid): this(uid, SubDevice.Root)
        {
        }

        public AbstractRDMDevice(UID uid, SubDevice subDevice)
        {
            this.uid = uid;
            this.subdevice = subDevice;
            subDevices = new List<IRDMDevice>();

            asyncRDMRequestHelper = new AsyncRDMRequestHelper(sendRDMRequestMessage);

            initialize();
        }

        private async void initialize()
        {
            if (!IsGenerated)
                await requestDeviceInfo();
        }

        #region Requests

        private async Task requestDeviceInfo()
        {
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, Subdevice, new ParameterBag(ERDM_Parameter.DEVICE_INFO));
            await runPeerToPeerProcess(ptpProcess);
            if (ptpProcess.ResponsePayloadObject.ParsedObject is RDMDeviceInfo deviceInfo)
            {
                DeviceInfo = deviceInfo;
                updateParameterValuesDependeciePropertyBag(ERDM_Parameter.DEVICE_INFO, ptpProcess.ResponsePayloadObject);
                updateParameterValuesDataTreeBranch(new ParameterDataCacheBag(ERDM_Parameter.DEVICE_INFO), ptpProcess.ResponsePayloadObject);
                await getDeviceModelAndCollectAllParameters();
            }
        }
        private async Task requestSensorValue(byte sensorId)
        {
            DataTreeBranch dataTreeBranch = new DataTreeBranch(new DataTree("sensor",0, sensorId));
            PeerToPeerProcess ptpProcess = new PeerToPeerProcess(ERDM_Command.GET_COMMAND, UID, Subdevice, new ParameterBag(ERDM_Parameter.SENSOR_VALUE), dataTreeBranch);
            await runPeerToPeerProcess(ptpProcess);
            if (ptpProcess.ResponsePayloadObject.ParsedObject is RDMSensorValue sensorValue)
            {
                //DeviceInfo = deviceInfo;
                //parameterValues.AddOrUpdate(ERDM_Parameter.DEVICE_INFO, deviceInfo, (o, p) => deviceInfo);
                //await getDeviceModelAndCollectAllParameters();
            }
        }
        private async Task requestParameters()
        {
            var parameters = this.DeviceModel?.SupportedNonBlueprintParameters;
            if (parameters == null)
                return;

            foreach (ERDM_Parameter parameter in parameters)
            {
                switch(parameter)
                {
                    case ERDM_Parameter.DEVICE_INFO:
                        continue;
                }
                ParameterBag parameterBag = new ParameterBag(parameter, this.DeviceModel.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                var define = MetadataFactory.GetDefine(parameterBag);
                if (define.GetRequest.HasValue)
                {
                    if (define.GetRequest.Value.GetIsEmpty())
                        await requestParameterWithEmptyPayload(parameterBag, define, UID, Subdevice);
                    else
                        await requestParameterWithPayload(parameterBag, define, UID, Subdevice);
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
                await collectAllParameters();
        }


        private protected void SetGeneratedParameterValue(ERDM_Parameter parameter, object value)
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
        private protected void SetGeneratedSensorDescription(RDMSensorDefinition value)
        {
            if (!IsGenerated)
                return;

            Sensor sensor = sensors.GetOrAdd(value.SensorId, (a) => new Sensor(a));
            sensor.UpdateDescription(value);
        }
        private protected void SetGeneratedSensorValue(RDMSensorValue value)
        {
            if (!IsGenerated)
                return;

            Sensor sensor = sensors.GetOrAdd(value.SensorId, (a) => new Sensor(a));
            sensor.UpdateValue(value);
        }
        public async Task<bool> SetParameter(ERDM_Parameter parameter, object value = null)
        {
            try
            {
                var pm = pmManager.GetRDMParameterWrapperByID(parameter);
                RDMMessage request = null;

                switch (pm)
                {
                    case IRDMSetParameterWrapperWithEmptySetRequest emptySetRequest:
                        request = emptySetRequest.BuildSetRequestMessage();
                        break;
                    case IRDMSetParameterWrapperRequest setRequest:
                        request = setRequest.BuildSetRequestMessage(value);
                        value = setRequest.SetRequestParameterDataToObject(request.ParameterData);
                        break;
                }

                if (request != null)
                {
                    var result = await requestParameter(request);
                    if (result.Success)
                        if (result.Response.ResponseType != ERDM_ResponseType.NACK_REASON)
                        {
                            parameterValues[parameter] = value;
                            return true;
                        }
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(string.Empty, e);
            }
            return false;
        }

        private async Task sendRDMRequestMessage(RDMMessage rdmMessage)
        {
            rdmMessage.DestUID = UID;
            await SendRDMMessage(rdmMessage);
        }

        protected abstract Task SendRDMMessage(RDMMessage rdmMessage);
        protected async Task ReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if (this.IsDisposed || IsDisposing)
                return;

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
                await SendRDMMessage(processRequestMessage(rdmMessage));
                return;
            }

            if (IsGenerated)
                return;

            if (rdmMessage.SourceUID != UID || !rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
                return;

            LastSeen = DateTime.UtcNow;

            if (asyncRDMRequestHelper.ReceiveMessage(rdmMessage))
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
            return await asyncRDMRequestHelper.RequestMessage(rdmMessage);
        }
        private async void DeviceModel_Initialized(object sender, EventArgs e)
        {
            deviceModel.Initialized -= DeviceModel_Initialized;
            await collectAllParameters();
        }
        private async Task collectAllParameters()
        {
            await requestParameters();
            //await UpdateParameterValues();
            //await UpdateSensorValues();
            //await UpdateSlotInfo();
            //await UpdateDefaultSlotValue();
            //await UpdateSlotDescriptions();
            AllDataPulled = true;
        }
        protected RDMMessage processRequestMessage(RDMMessage rdmMessage)
        {
            var pm = pmManager.GetRDMParameterWrapperByID(rdmMessage.Parameter);
            RDMMessage response = null;
            try
            {
                if (rdmMessage.Command == ERDM_Command.DISCOVERY_COMMAND)
                {
                    switch (rdmMessage.Parameter)
                    {
                        case ERDM_Parameter.DISC_MUTE:
                            DiscoveryMuted = true;
                            response = new RDMMessage
                            {
                                Parameter = ERDM_Parameter.DISC_MUTE,
                                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                                SourceUID = UID,
                                DestUID = rdmMessage.SourceUID,
                                ParameterData = new DiscMuteUnmuteResponse().ToPayloadData()
                            };
                            return rdmMessage.DestUID != UID.Broadcast ? response : null;
                        case ERDM_Parameter.DISC_UN_MUTE:
                            DiscoveryMuted = false;
                            response = new RDMMessage
                            {
                                Parameter = ERDM_Parameter.DISC_UN_MUTE,
                                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                                SourceUID = UID,
                                DestUID = rdmMessage.SourceUID,
                                ParameterData = new DiscMuteUnmuteResponse().ToPayloadData()
                            };
                            return rdmMessage.DestUID != UID.Broadcast ? response : null;
                        case ERDM_Parameter.DISC_UNIQUE_BRANCH when !DiscoveryMuted && rdmMessage.Value is DiscUniqueBranchRequest discUniqueBranchRequest:
                            if (UID >= discUniqueBranchRequest.StartUid && UID <= discUniqueBranchRequest.EndUid)
                            {
                                response = new RDMMessage
                                {
                                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                                    Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                                    SourceUID = UID
                                };
                                return response;
                            }
                            return null;
                    }
                }
                if (rdmMessage.Command == ERDM_Command.GET_COMMAND)
                {
                    ConcurrentDictionary<object, object> list = null;
                    object value = null;
                    object index = null;
                    parameterValues.TryGetValue(rdmMessage.Parameter, out object responseValue);
                    switch (pm)
                    {
                        case DeviceInfoParameterWrapper _deviceInfoParameterWrapper:
                            response = _deviceInfoParameterWrapper.BuildGetResponseMessage(DeviceInfo);
                            break;

                        case SupportedParametersParameterWrapper _supportedParametersParameterWrapper:
                            List<ERDM_Parameter> sp = new List<ERDM_Parameter>();
                            sp.Add(ERDM_Parameter.DEVICE_INFO);
                            sp.AddRange(parameterValues.Keys);
                            if (deviceModel != null)
                                sp.AddRange(deviceModel.SupportedParameters);
                            if (!sensors.IsEmpty)
                            {
                                sp.Add(ERDM_Parameter.SENSOR_DEFINITION);
                                sp.Add(ERDM_Parameter.SENSOR_VALUE);
                                if(sensors.Any(s=>s.Value.RecordedValueSupported))
                                    sp.Add(ERDM_Parameter.RECORD_SENSORS);
                            }
                            response = _supportedParametersParameterWrapper.BuildGetResponseMessage(sp.Distinct().ToArray());
                            break;
                        case QueuedMessageParameterWrapper _queuedMessageParameterWrapper:
                            response = statusMessageParameterWrapper.BuildGetResponseMessage([]);
                            break;
                        case SlotInfoParameterWrapper slotInfoParameterWrapper:
                            list = null;
                            if (parameterValues.TryGetValue(rdmMessage.Parameter, out value) && value is RDMSlotInfo[] slotInfos)
                                response = slotInfoParameterWrapper.BuildGetResponseMessage(slotInfos);
                            break;
                        case DefaultSlotValueParameterWrapper defaultSlotValueParameterWrapper:
                            list = null;
                            if (parameterValues.TryGetValue(rdmMessage.Parameter, out value) && value is RDMDefaultSlotValue[] defaultValues)
                                response = defaultSlotValueParameterWrapper.BuildGetResponseMessage(defaultValues);
                            break;

                        case IRDMDescriptionParameterWrapper _descriptionParameterWrapper:
                            index = _descriptionParameterWrapper.GetRequestParameterDataToObject(rdmMessage.ParameterData);
                            if (index == null)
                                break;

                            list = null;
                            if (parameterValues.TryGetValue(rdmMessage.Parameter, out value))
                                list = value as ConcurrentDictionary<object, object>;

                            if (list == null && !IsGenerated)
                                parameterValues[rdmMessage.Parameter] = DeviceModel.ParameterValues[rdmMessage.Parameter];

                            if (list == null)
                                break;

                            list.TryGetValue(index, out responseValue);
                            if (responseValue == null)
                                break;
                            response = _descriptionParameterWrapper.BuildGetResponseMessage(responseValue);
                            break;

                        case DMX512StartingAddressParameterWrapper dmx512StartingAddressParameterWrapper
                            when responseValue is ushort _ushort:
                            response = dmx512StartingAddressParameterWrapper.BuildGetResponseMessage(_ushort);
                            break;
                        case SensorValueParameterWrapper sensorValueParameterWrapper:
                            if (sensors.TryGetValue((byte)rdmMessage.Value, out Sensor sensor))
                                response = sensorValueParameterWrapper.BuildGetResponseMessage((RDMSensorValue)sensor);
                            break;

                        case IRDMGetParameterWrapperResponse getParameterWrapperResponse:
                            response = getParameterWrapperResponse.BuildGetResponseMessage(responseValue);
                            break;
                    }
                }
                else if (rdmMessage.Command == ERDM_Command.SET_COMMAND)
                {
                    bool success = false;
                    //Handle set Requerst
                    switch (pm)
                    {
                        case IRDMSetParameterWrapperRequestContravariance<bool> setParameterWrapperRequestContravarianceBool
                        when setParameterWrapperRequestContravarianceBool.SetRequestParameterDataToObject(rdmMessage.ParameterData) is bool _bool:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _bool);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<string> setParameterWrapperRequestContravarianceString
                        when setParameterWrapperRequestContravarianceString.SetRequestParameterDataToObject(rdmMessage.ParameterData) is string _string:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _string);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<byte> setParameterWrapperRequestContravarianceByte
                        when setParameterWrapperRequestContravarianceByte.SetRequestParameterDataToObject(rdmMessage.ParameterData) is byte _byte:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _byte);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<sbyte> setParameterWrapperRequestContravarianceSByte
                        when setParameterWrapperRequestContravarianceSByte.SetRequestParameterDataToObject(rdmMessage.ParameterData) is sbyte _sbyte:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _sbyte);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<short> setParameterWrapperRequestContravarianceShort
                        when setParameterWrapperRequestContravarianceShort.SetRequestParameterDataToObject(rdmMessage.ParameterData) is short _short:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _short);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<ushort> setParameterWrapperRequestContravarianceUShort
                        when setParameterWrapperRequestContravarianceUShort.SetRequestParameterDataToObject(rdmMessage.ParameterData) is ushort _ushort:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _ushort);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<int> setParameterWrapperRequestContravarianceInt
                        when setParameterWrapperRequestContravarianceInt.SetRequestParameterDataToObject(rdmMessage.ParameterData) is int _int:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _int);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<uint> setParameterWrapperRequestContravarianceUInt
                        when setParameterWrapperRequestContravarianceUInt.SetRequestParameterDataToObject(rdmMessage.ParameterData) is uint _uint:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _uint);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<long> setParameterWrapperRequestContravarianceLong
                        when setParameterWrapperRequestContravarianceLong.SetRequestParameterDataToObject(rdmMessage.ParameterData) is long _long:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _long);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<ulong> setParameterWrapperRequestContravarianceULong
                        when setParameterWrapperRequestContravarianceULong.SetRequestParameterDataToObject(rdmMessage.ParameterData) is ulong _ulong:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _ulong);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<double> setParameterWrapperRequestContravarianceDouble
                        when setParameterWrapperRequestContravarianceDouble.SetRequestParameterDataToObject(rdmMessage.ParameterData) is double _double:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _double);
                            break;

                        case IRDMSetParameterWrapperRequestContravariance<float> setParameterWrapperRequestContravarianceFloat
                        when setParameterWrapperRequestContravarianceFloat.SetRequestParameterDataToObject(rdmMessage.ParameterData) is float _float:
                            success = updateParametrerValueCache(rdmMessage.Parameter, _float);
                            break;
                    }
                    if (!success)
                    {
                        response = new RDMMessage(ERDM_NackReason.FORMAT_ERROR) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                        goto FAIL;
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
            }
            catch (Exception e)
            {
                Logger?.LogError(e, string.Empty);
            }
        FAIL:

            if (rdmMessage.DestUID.IsBroadcast) // no Response on Broadcast
                return null;

            response ??= new RDMMessage(ERDM_NackReason.UNKNOWN_PID) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };

            response.TransactionCounter = rdmMessage.TransactionCounter;
            response.SourceUID = rdmMessage.DestUID;
            response.DestUID = rdmMessage.SourceUID;
            return response;
        }

        //private async Task processResponseMessage(RequestResult result)
        //{
        //    if (IsGenerated)
        //        return;

        //    if (result.Success)
        //        await processResponseMessage(result.Response);
        //    else if (result.Cancel)
        //        return;
        //    else
        //    {
        //        await Task.Delay(TimeSpan.FromTicks(random.Next(4500, 5500)));
        //        await processResponseMessage(await requestParameter(result.Request));
        //    }
        //}
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
                Logger?.LogError(ex);
            }
            if (value == null)
                return;
            ConcurrentDictionary<object, object> list = null;
            switch (pm)
            {
                case DeviceInfoParameterWrapper _deviceInfoParameterWrapper:
                    if (rdmMessage.Value is not RDMDeviceInfo deviceInfo)
                        break;

                    DeviceInfo = deviceInfo;

                    await getDeviceModelAndCollectAllParameters();

                    break;

                case SlotDescriptionParameterWrapper _slotDescriptionParameterWrapper:
                    if (value is not RDMSlotDescription description)
                    {
                        if (value != null)
                            Logger?.LogError($"The response does not contain the expected data {typeof(RDMSlotDescription)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger?.LogTrace($"No response received");
                        return;
                    }


                    if (parameterValues.TryGetValue(rdmMessage.Parameter, out value))
                        list = value as ConcurrentDictionary<object, object>;
                    if (list == null)
                        parameterValues[rdmMessage.Parameter] = list = new ConcurrentDictionary<object, object>();
                    list.AddOrUpdate(description.SlotId, description, (e, f) => description);
                    Slot slot;
                    if (!slots.TryGetValue(description.SlotId, out slot))
                    {
                        slot = new Slot(description.SlotId);
                        slots.TryAdd(slot.SlotId, slot);
                    }
                    slot.UpdateSlotDescription(description);
                    this.OnPropertyChanged(nameof(this.Slots));
                    break;

                case SlotInfoParameterWrapper _slotInfoParameterWrapper:
                    if (value is not RDMSlotInfo[] slotInfos)
                    {
                        if (rdmMessage.NackReason.Contains(ERDM_NackReason.ACTION_NOT_SUPPORTED))
                            this.slots = null; //Set to null, to Deactivate this UpdateSlotInfo

                        Logger?.LogError($"The response does not contain the expected data {typeof(RDMSlotInfo[])}!{Environment.NewLine}{rdmMessage}");
                        return;
                    }

                    parameterValues[rdmMessage.Parameter] = value;

                    foreach (RDMSlotInfo info in slotInfos)
                        Slots[info.SlotOffset].UpdateSlotInfo(info);

                    this.OnPropertyChanged(nameof(this.Slots));
                    break;

                case DefaultSlotValueParameterWrapper _defaultSlotValueParameterWrapper:
                    if (value is not RDMDefaultSlotValue[] defaultSlotValues)
                    {
                        Logger?.LogError($"The response does not contain the expected data {typeof(RDMDefaultSlotValue[])}!{Environment.NewLine}{rdmMessage}");
                        return;
                    }

                    parameterValues[rdmMessage.Parameter] = defaultSlotValues;

                    foreach (RDMDefaultSlotValue info in defaultSlotValues)
                        Slots[info.SlotOffset].UpdateSlotDefaultValue(info);

                    this.OnPropertyChanged(nameof(this.Slots));
                    break;
                case SensorValueParameterWrapper _sensorValueParameterWrapper:
                    if (value is not RDMSensorValue sensorValue)
                    {
                        if (value != null)
                            Logger?.LogError($"The response does not contain the expected data {typeof(RDMSensorDefinition)}!{Environment.NewLine}{rdmMessage}");
                        else
                            Logger?.LogError($"No response received");
                        return;
                    }
                    Sensor sensorV = sensors.GetOrAdd(sensorValue.SensorId, (a) => new Sensor(a));
                    sensorV.UpdateValue(sensorValue);
                    this.OnPropertyChanged(nameof(this.Sensors));
                    break;

                case IRDMGetParameterWrapperWithEmptyGetRequest @emptyGetRequest:
                    updateParametrerValueCache(rdmMessage.Parameter, value);
                    break;

                case IRDMGetParameterWrapperRequest<byte> @emptyGetRequest:
                    updateParametrerValueCache(rdmMessage.Parameter, value);
                    break;
                case IRDMGetParameterWrapperRequest<ushort> @emptyGetRequest:
                    updateParametrerValueCache(rdmMessage.Parameter, value);
                    break;
                case IRDMGetParameterWrapperRequest<uint> @emptyGetRequest:
                    updateParametrerValueCache(rdmMessage.Parameter, value);
                    break;

                default:
                    break;

            }
        }

        protected virtual void OnPropertyChanged(string property)
        {
            this.PropertyChanged.InvokeFailSafe(this, new PropertyChangedEventArgs(property));
        }

        //public async Task UpdateParameterValues()
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceModel == null)
        //        return;

        //    try
        //    {
        //        foreach (ERDM_Parameter parameter in this.DeviceModel.SupportedNonBlueprintParameters)
        //            await this.UpdateParameterValue(parameter);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to get UpdateParameterValues for UID: {this.UID}", e);
        //    }
        //}
        //public async Task UpdateParameterValue(ERDM_Parameter parameterId)
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceModel == null)
        //        return;
        //    if (this.pendingParametersUpdateRequest.Contains(parameterId))
        //        return;

        //    switch (parameterId)
        //    {
        //        case ERDM_Parameter.SENSOR_DEFINITION:
        //        case ERDM_Parameter.SENSOR_VALUE:
        //        case ERDM_Parameter.SLOT_INFO:
        //        case ERDM_Parameter.SLOT_DESCRIPTION:
        //        case ERDM_Parameter.DEFAULT_SLOT_VALUE:
        //        case ERDM_Parameter.QUEUED_MESSAGE:
        //            return;
        //    }

        //    var pm = pmManager.GetRDMParameterWrapperByID(parameterId);
        //    if (pm == null && Enum.IsDefined(typeof(ERDM_Parameter), parameterId))
        //        return;


        //    if (!pm.CommandClass.HasFlag(ERDM_CommandClass.GET))
        //        return;

        //    if (pm is IRDMBlueprintParameterWrapper)
        //        return;

        //    this.pendingParametersUpdateRequest.Add(parameterId);
        //    try
        //    {
        //        List<Task> tasks = new List<Task>();
        //        object val = null;
        //        switch (pm)
        //        {
        //            case IRDMGetParameterWrapperWithEmptyGetRequest @emptyGetRequest:
        //                tasks.Add(processResponseMessage(await requestParameter(@emptyGetRequest.BuildGetRequestMessage())));
        //                break;
        //            case IRDMGetParameterWrapperRequestRanged<byte> @byteGetRequest:
        //                foreach (ERDM_Parameter para in @byteGetRequest.DescriptiveParameters)
        //                {
        //                    this.DeviceModel.ParameterValues.TryGetValue(para, out val);
        //                    if (val != null)
        //                        break;
        //                }
        //                foreach (var r in @byteGetRequest.GetRequestRange(val).ToEnumerator())
        //                    tasks.Add(processResponseMessage(await requestParameter(@byteGetRequest.BuildGetRequestMessage(r))));

        //                break;
        //            case IRDMGetParameterWrapperRequestRanged<ushort> @ushortGetRequest:
        //                foreach (ERDM_Parameter para in @ushortGetRequest.DescriptiveParameters)
        //                {
        //                    this.DeviceModel.ParameterValues.TryGetValue(para, out val);
        //                    if (val != null)
        //                        break;
        //                }
        //                foreach (var r in @ushortGetRequest.GetRequestRange(val).ToEnumerator())
        //                    tasks.Add(processResponseMessage(await requestParameter(@ushortGetRequest.BuildGetRequestMessage(r))));

        //                break;
        //            case IRDMGetParameterWrapperRequestRanged<uint> @uintGetRequest:
        //                foreach (ERDM_Parameter para in @uintGetRequest.DescriptiveParameters)
        //                {
        //                    this.DeviceModel.ParameterValues.TryGetValue(para, out val);
        //                    if (val != null)
        //                        break;
        //                }
        //                foreach (var r in @uintGetRequest.GetRequestRange(val).ToEnumerator())
        //                    tasks.Add(processResponseMessage(await requestParameter(@uintGetRequest.BuildGetRequestMessage(r))));

        //                break;

        //            case StatusMessageParameterWrapper statusMessageParameter:
        //                tasks.Add(processResponseMessage(await requestParameter(statusMessageParameter.BuildGetRequestMessage(ERDM_Status.ADVISORY))));
        //                break;
        //            default:
        //                Logger?.LogDebug($"No Wrapper for Parameter: {parameterId} for UID: {this.UID}");
        //                break;
        //        }
        //        await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update ParameterValue of Parameter: {parameterId} for UID: {this.UID}", e);
        //    }
        //    this.pendingParametersUpdateRequest.Remove(parameterId);
        //}
        //public async Task UpdateSensorValues()
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null)
        //        return;

        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SENSOR_DEFINITION) != true)
        //        return;

        //    if (this.DeviceModel == null)
        //        return;

        //    if (this.DeviceInfo.SensorCount == 0)
        //        return;

        //    try
        //    {
        //        var sensorDefenitions = this.DeviceModel.GetSensorDefinitions();
        //        if (sensorDefenitions == null)
        //            return;

        //        List<Task> tasks = new List<Task>();
        //        foreach (var sd in sensorDefenitions)
        //        {
        //            sensors.GetOrAdd(sd.SensorId, (a) =>
        //            {
        //                var s = new Sensor(a);
        //                s.UpdateDescription(sd);
        //                return s;
        //            });

        //            tasks.Add(this.UpdateSensorValue(sd.SensorId));
        //        }

        //        await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update SensorValues for UID: {this.UID}", e);
        //    }
        //}
        //public async Task UpdateSensorValue(byte sensorId)
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null)
        //        return;
        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SENSOR_VALUE) != true)
        //        return;
        //    if (this.pendingSensorValuesUpdateRequest.Contains(sensorId))
        //        return;

        //    if (this.DeviceInfo.SensorCount == 0)
        //        return;

        //    try
        //    {
        //        this.pendingSensorValuesUpdateRequest.Add(sensorId);
        //        await processResponseMessage(await requestParameter(sensorValueParameterWrapper.BuildGetRequestMessage(sensorId)));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update SensorValue of Sensor: {sensorId} for UID: {this.UID}", e);
        //    }
        //    this.pendingSensorValuesUpdateRequest.Remove(sensorId);
        //}
        //public async Task UpdateSlotInfo()
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null || this.slots == null)
        //        return;

        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_INFO) != true)
        //        return;

        //    try
        //    {
        //        RequestResult? result = null;
        //        do
        //        {
        //            result = await requestParameter(slotInfoParameterWrapper.BuildGetRequestMessage());
        //            if (result.Value.Success)
        //                await processResponseMessage(result.Value.Response);
        //            else if (result.Value.Cancel)
        //                return;
        //            else
        //                await Task.Delay(TimeSpan.FromTicks(random.Next(2500, 3500)));
        //        }
        //        while (result?.Response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW || result?.Response == null);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update SlotInfo for UID: {this.UID}", e);
        //    }
        //}
        //public async Task UpdateDefaultSlotValue()
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null || this.slots == null)
        //        return;

        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.DEFAULT_SLOT_VALUE) != true)
        //        return;

        //    try
        //    {
        //        RequestResult? result = null;
        //        do
        //        {
        //            result = await requestParameter(defaultSlotValueParameterWrapper.BuildGetRequestMessage());
        //            if (result.Value.Success)
        //                await processResponseMessage(result.Value.Response);
        //            else if (result.Value.Cancel)
        //                return;
        //            else
        //                await Task.Delay(TimeSpan.FromTicks(random.Next(2500, 3500)));
        //        }
        //        while (result?.Response?.ResponseType == ERDM_ResponseType.ACK_OVERFLOW || result?.Response == null);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update DefaultSlotValue for UID: {this.UID}", e);
        //    }
        //}
        //public async Task UpdateSlotDescriptions()
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null)
        //        return;

        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_DESCRIPTION) != true)
        //        return;

        //    if (this.Slots.Count == 0)
        //        return;

        //    try
        //    {
        //        List<Task> tasks = new List<Task>();
        //        foreach (var slot in this.slots)
        //            tasks.Add(this.UpdateSlotDescription(slot.Key));

        //        await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromSeconds(10)));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update SlotDescriptions for UID: {this.UID}", e);
        //    }
        //}
        //public async Task UpdateSlotDescription(ushort slotId)
        //{
        //    if (IsGenerated)
        //        return;

        //    if (this.DeviceInfo == null)
        //        return;
        //    if (this.DeviceModel?.SupportedParameters?.Contains(ERDM_Parameter.SLOT_DESCRIPTION) != true)
        //        return;

        //    if (this.pendingSlotDescriptionsUpdateRequest.Contains(slotId))
        //        return;

        //    if (this.Slots.Count == 0)
        //        return;

        //    try
        //    {
        //        this.pendingSlotDescriptionsUpdateRequest.Add(slotId);
        //        await processResponseMessage(await requestParameter(slotDescriptionParameterWrapper.BuildGetRequestMessage(slotId)));
        //    }
        //    catch (Exception e)
        //    {
        //        Logger?.LogError($"Not able to update SlotDescription of Slot: {slotId} for UID: {this.UID}", e);
        //    }
        //    this.pendingSlotDescriptionsUpdateRequest.Remove(slotId);
        //}

        private bool updateParametrerValueCache(ERDM_Parameter parameter, object value)
        {
            parameterValues.AddOrUpdate(parameter, value, (x, y) => value);
            return OnUpdateParametrerValueCache(parameter, value);
        }
        protected virtual bool OnUpdateParametrerValueCache(ERDM_Parameter parameter, object value)
        {
            return true;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose-Methoden müssen SuppressFinalize aufrufen", Justification = "<Ausstehend>")]
        public new void Dispose()
        {
            if (IsDisposing || IsDisposed)
                return;
            IsDisposing = true;
            asyncRDMRequestHelper.Dispose();
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