using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static RDMSharp.RDMSharp;

[assembly: InternalsVisibleTo("RDMSharpTests")]
namespace RDMSharp
{
    public abstract class AbstractGeneratedRDMDevice : AbstractRDMDevice
    {
        public sealed override bool IsGenerated => true;
        public abstract bool SupportQueued { get; }
        public abstract bool SupportStatus { get; }
        #region DeviceInfoStuff
        private HashSet<ERDM_Parameter> _parameters;
        public IReadOnlySet<ERDM_Parameter> Parameters { get => _parameters;
            private set
            {
                if (_parameters == value)
                    return;
                _parameters = value.ToHashSet();
                this.OnPropertyChanged();
            }
        }
        public abstract EManufacturer ManufacturerID { get; }
        public abstract ushort DeviceModelID { get; }
        public abstract ERDM_ProductCategoryCoarse ProductCategoryCoarse { get; }
        public abstract ERDM_ProductCategoryFine ProductCategoryFine { get; }
        public uint SoftwareVersionID
        {
            get
            {
                var softwareVersionModule = this.Modules.OfType<SoftwareVersionModule>().FirstOrDefault();
                return softwareVersionModule?.SoftwareVersionId ?? 0;
            }
        }
        #endregion
        public IReadOnlyCollection<GeneratedPersonality> Personalities
        {
            get
            {
                return dmxPersonalityModule?.Personalities ?? Array.Empty<GeneratedPersonality>();
            }
        }

        public sealed override IReadOnlyDictionary<byte, Sensor> Sensors { get { return sensorsModule?.Sensors; } }

        public sealed override IReadOnlyDictionary<ushort, Slot> Slots { get { return slotsModule?.Slots; } }

        private ConcurrentDictionary<int, RDMStatusMessage> statusMessages = new ConcurrentDictionary<int, RDMStatusMessage>();
        public sealed override IReadOnlyDictionary<int, RDMStatusMessage> StatusMessages { get { return statusMessages.AsReadOnly(); } }
        private ConcurrentDictionary<UID, ControllerCommunicationCache> controllerCommunicationCache = new ConcurrentDictionary<UID, ControllerCommunicationCache>();

        private RDMDeviceInfo deviceInfo;
        public sealed override RDMDeviceInfo DeviceInfo { get { return deviceInfo; } }

        private ConcurrentDictionary<UID, OverflowCacheBag> overflowCacheBags = new ConcurrentDictionary<UID, OverflowCacheBag>();

        private readonly DeviceInfoModule deviceInfoModule;
        private readonly IdentifyDeviceModule identifyDeviceModule;
        private readonly DMX_StartAddressModule? dmxStartAddressModule;
        private readonly DMX_PersonalityModule? dmxPersonalityModule;
        private readonly SlotsModule? slotsModule;
        private readonly SensorsModule? sensorsModule;

        private readonly IReadOnlyCollection<IModule> _modules;
        public IReadOnlyCollection<IModule> Modules { get => _modules; }

        public ushort? DMXAddress
        {
            get
            {
                if (dmxStartAddressModule is null)
                    return null;

                return dmxStartAddressModule.DMXAddress;
            }
            set
            {
                if (dmxStartAddressModule is null)
                    return;
                if (dmxStartAddressModule.DMXAddress == value)
                    return;

                dmxStartAddressModule.DMXAddress = value;
                this.OnPropertyChanged(nameof(this.DMXAddress));
            }
        }

        public byte? CurrentPersonality
        {
            get
            {
                return dmxPersonalityModule.CurrentPersonality;
            }
            set
            {
                if (dmxPersonalityModule is null)
                    return;
                if (dmxPersonalityModule.CurrentPersonality == value)
                    return;

                dmxPersonalityModule.CurrentPersonality = value;
                this.OnPropertyChanged(nameof(this.CurrentPersonality));
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

        public bool Identify
        {
            get
            {
                return identifyDeviceModule.Identify;
            }
            set
            {
                if (string.Equals(identifyDeviceModule.Identify, value))
                    return;

                identifyDeviceModule.Identify = value;
                this.OnPropertyChanged();
            }
        }

        private bool _initialized = false;

        protected AbstractGeneratedRDMDevice(UID uid, ERDM_Parameter[] parameters, IRDMDevice[] subDevices = null, IReadOnlyCollection<IModule> modules = null) : this(uid, SubDevice.Root, parameters, subDevices, modules)
        {
        }
        protected AbstractGeneratedRDMDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, IReadOnlyCollection<IModule> modules =null) : this(uid, subDevice, parameters, null, modules)
        {
        }
        private AbstractGeneratedRDMDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, IRDMDevice[] subDevices = null, IReadOnlyCollection<IModule> modules = null) : base(uid, subDevice, subDevices)
        {
            if (!((ushort)ManufacturerID).Equals(uid.ManufacturerID))
                throw new Exception($"{uid.ManufacturerID} not match the {ManufacturerID}");

            RDMSharp.Instance.RequestReceivedEvent += Instance_RequestReceivedEvent;

            List<IModule> moduleList = new List<IModule>();
            identifyDeviceModule = new IdentifyDeviceModule();
            moduleList.Add(identifyDeviceModule);

            if (modules is not null)
                moduleList.AddRange(modules);

            deviceInfoModule = new DeviceInfoModule();
            moduleList.Add(deviceInfoModule);
            _modules = moduleList.AsReadOnly();
            dmxStartAddressModule = _modules.OfType<DMX_StartAddressModule>().FirstOrDefault();
            dmxPersonalityModule = _modules.OfType<DMX_PersonalityModule>().FirstOrDefault();
            slotsModule = _modules.OfType<SlotsModule>().FirstOrDefault();
            sensorsModule = _modules.OfType<SensorsModule>().FirstOrDefault();
            if (dmxPersonalityModule is not null)//Remove after Refactoring to Modules
                dmxPersonalityModule.PropertyChanged += DmxPersonalityModule_PropertyChanged;


                #region Parameters
            var _params = new HashSet<ERDM_Parameter>();
            if (parameters != null && parameters.Length != 0)
                foreach (var p in parameters)
                    _params.Add(p);

            if (SupportQueued)
                _params.Add(ERDM_Parameter.QUEUED_MESSAGE);
            if (SupportStatus)
            {
                _params.Add(ERDM_Parameter.STATUS_MESSAGES);
                _params.Add(ERDM_Parameter.CLEAR_STATUS_ID);
            }

            _params.Add(ERDM_Parameter.DEVICE_INFO);
            _params.Add(ERDM_Parameter.SUPPORTED_PARAMETERS);

            foreach (IModule module in _modules)
                foreach (var parameter in module.SupportedParameters)
                    _params.Add(parameter);

            Parameters = _params;

            foreach (IModule module in _modules)
                if (module is AbstractModule aModule)
                    aModule.SetParentDevice(this);

            trySetParameter(ERDM_Parameter.SUPPORTED_PARAMETERS, Parameters.ToArray());
            #endregion

            #region StatusMessage
            if (SupportStatus)
                trySetParameter(ERDM_Parameter.STATUS_MESSAGES, new RDMStatusMessage[0]);
            #endregion

            updateDeviceInfo();
            _initialized = true;
        }

        private void DmxPersonalityModule_PropertyChanged(object sender, PropertyChangedEventArgs e)//Remove after Refactoring to Modules
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void updateDeviceInfo()
        {
            var info = new RDMDeviceInfo(1,
                                           0,
                                           DeviceModelID,
                                           ProductCategoryCoarse,
                                           ProductCategoryFine,
                                           SoftwareVersionID,
                                           dmx512Footprint: dmxPersonalityModule?.CurrentPersonalityFootprint ?? 0,
                                           dmx512CurrentPersonality: dmxPersonalityModule?.CurrentPersonality ?? 0,
                                           dmx512NumberOfPersonalities: dmxPersonalityModule?.PersonalitiesCount ?? 0,
                                           dmx512StartAddress: dmxStartAddressModule?.DMXAddress ?? ushort.MaxValue,
                                           subDeviceCount: (ushort)(SubDevices?.Where(sd => !sd.Subdevice.IsRoot).Count() ?? 0),
                                           sensorCount: (byte)(Sensors?.Count ?? 0));
            updateDeviceInfo(info);
        }

        private void updateDeviceInfo(RDMDeviceInfo value)
        {
            if (RDMDeviceInfo.Equals(deviceInfo, value))
                return;

            deviceInfo = value;
            this.OnPropertyChanged(nameof(this.DeviceInfo));
        }

        private void updateSupportedParametersOnAddRemoveSensors()
        {
            var oldParameters = Parameters;

            if (!Parameters.SequenceEqual(oldParameters))
                trySetParameter(ERDM_Parameter.SUPPORTED_PARAMETERS, Parameters.ToArray());
        }

        

        protected bool trySetParameter(ERDM_Parameter parameter, object value)
        {
            if (!this.Parameters.Contains(parameter))
                throw new NotSupportedException($"The Parameter: {parameter}, is not Supported");

            setParameterValue(parameter, value);
            return true;
        }
        public bool TrySetParameter(ERDM_Parameter parameter, object value, bool throwException = true)
        {

            if (!this.Parameters.Contains(parameter))
            {
                if (throwException)
                    throw new NotSupportedException($"The Device not support the Parameter: {parameter}");
                return false;
            }

            switch (parameter)
            {
                case ERDM_Parameter.DMX_START_ADDRESS:
                case ERDM_Parameter.DEVICE_LABEL:
                    if (throwException)
                        throw new NotSupportedException($"You have no permission to set the Parameter: {parameter}, use the public Propertys to set them");
                    return false;

                case ERDM_Parameter.DEVICE_INFO:
                case ERDM_Parameter.DEVICE_MODEL_DESCRIPTION:
                case ERDM_Parameter.MANUFACTURER_LABEL:
                case ERDM_Parameter.QUEUED_MESSAGE:
                case ERDM_Parameter.SUPPORTED_PARAMETERS:
                case ERDM_Parameter.SLOT_DESCRIPTION:
                case ERDM_Parameter.SLOT_INFO:
                case ERDM_Parameter.DEFAULT_SLOT_VALUE:
                case ERDM_Parameter.SENSOR_DEFINITION:
                case ERDM_Parameter.SENSOR_VALUE:
                    if (throwException)
                        throw new NotSupportedException($"The Protocoll not allow to set the Parameter: {parameter}");
                    return false;
            }

            var parameterBag = new ParameterBag(parameter, UID.ManufacturerID, DeviceInfo?.DeviceModelId, DeviceInfo?.SoftwareVersionId);
            var define = MetadataFactory.GetDefine(parameterBag);
            if (define != null)
            {
                if (!define.SetRequest.HasValue)
                {
                    if (throwException)
                        throw new NotSupportedException($"The Protocoll not allow to set the Parameter: {parameter}");
                    return false;
                }

                try
                {
                    if (!define.SetRequest.HasValue)
                        throw new NotSupportedException($"The Protocoll not allow to set the Parameter: {parameter}");
                    else
                    {
                        byte[] data = MetadataFactory.ParsePayloadToData(define, Metadata.JSON.Command.ECommandDublicate.SetRequest, DataTreeBranch.FromObject(value, null, parameterBag, ERDM_Command.SET_COMMAND)).First();
                        var obj = MetadataFactory.ParseDataToPayload(define, Metadata.JSON.Command.ECommandDublicate.SetRequest, data);
                        if (!object.Equals(value, obj))
                            return false;
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, string.Empty);
                    return false;
                }
            }

            return this.trySetParameter(parameter, value);
        }
        protected sealed override void OnPropertyChanged([CallerMemberName]string property=null)
        {
            switch (property)
            {
                case nameof(DeviceInfo):
                    trySetParameter(ERDM_Parameter.DEVICE_INFO, this.DeviceInfo);
                    trySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, this.DeviceInfo.SoftwareVersionId);
                    break;
                //case nameof(CurrentPersonality):
                //    var slots = Personalities.First(p => p.ID == this.CurrentPersonality).Slots.Count;
                //    var slotInfos = new RDMSlotInfo[slots];
                //    var slotDesc = new ConcurrentDictionary<object, object>();
                //    var slotDefault = new RDMDefaultSlotValue[slots];
                //    foreach (var s in Personalities.First(p => p.ID == this.CurrentPersonality).Slots)
                //    {
                //        Slot slot = s.Value;
                //        slotInfos[slot.SlotId] = new RDMSlotInfo(slot.SlotId, slot.Type, slot.Category);
                //        slotDesc.TryAdd(slot.SlotId, new RDMSlotDescription(slot.SlotId, slot.Description));
                //        slotDefault[slot.SlotId] = new RDMDefaultSlotValue(slot.SlotId, slot.DefaultValue);
                //    }
                //    trySetParameter(ERDM_Parameter.SLOT_INFO, slotInfos);
                //    trySetParameter(ERDM_Parameter.SLOT_DESCRIPTION, slotDesc);
                //    trySetParameter(ERDM_Parameter.DEFAULT_SLOT_VALUE, slotDefault);
                //    break;
            }
            base.OnPropertyChanged(property);
        }


        public bool SetParameter(ERDM_Parameter parameter, object value = null)
        {
            setParameterValue(parameter, value);
            return true;
        }
        internal void setParameterValue(ERDM_Parameter parameter, object value, object index=null)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_INFO when value is RDMDeviceInfo _deviceInfo:
                    deviceInfo = _deviceInfo;
                    goto default;

                default:
                    bool notNew = false;
                    if (value is null)
                    {
                        parameterValues.TryRemove(parameter, out object oldValue);
                        return;
                    }
                    bool raiseAddedEvent = false;
                    bool raiseUpdatedEvent = false;
                    object? ov = null;
                    parameterValues.AddOrUpdate(parameter, (_) =>
                    {
                        try
                        {
                            return value;
                        }
                        finally
                        {
                            raiseAddedEvent = true;
                        }
                    }, (o, p) =>
                    {
                        try
                        {
                            ov = p;
                            if (object.Equals(ov, value) && value is not ConcurrentDictionary<object, object>)
                                notNew = true;
                            return value;
                        }
                        finally
                        {
                            raiseUpdatedEvent = true;
                        }
                    });

                    try
                    {
                        if (notNew)
                            return;
                        if (parameter != ERDM_Parameter.SLOT_DESCRIPTION)
                        {
                            updateParameterBag(parameter, index);
                            return;
                        }
                        if (value is ConcurrentDictionary<object, object> dict)
                        {
                            foreach (var p in dict)
                                updateParameterBag(parameter, p.Key);
                        }
                        return;
                    }
                    finally
                    {
                        if (raiseAddedEvent)
                            InvokeParameterValueAdded(new ParameterValueAddedEventArgs(parameter, value, index));
                        if (raiseUpdatedEvent)
                            InvokeParameterValueChanged(new ParameterValueChangedEventArgs(parameter, value, ov, index));
                    }
            }
        }


        #region SendReceive Pipeline
        private void Instance_RequestReceivedEvent(object sender, RequestReceivedEventArgs e)
        {
            RDMMessage response = null;
            if ((e.Request.DestUID.IsBroadcast || e.Request.DestUID == UID) && !e.Request.Command.HasFlag(ERDM_Command.RESPONSE))
            {
                if (e.Request.SubDevice.IsBroadcast)
                {
                    foreach (var sd in this.SubDevices)
                        _ = processRequestMessage(e.Request); //Drop response on Broadcast
                    return;
                }

                AbstractGeneratedRDMDevice sds = null;
                if (e.Request.SubDevice == this.Subdevice)
                    sds = this;
                else
                    sds = this.SubDevices?.OfType<AbstractGeneratedRDMDevice>().FirstOrDefault(sd => sd.Subdevice == e.Request.SubDevice);

                if (sds != null)
                    response = sds.processRequestMessage(e.Request);
            }
            if (e.Request.Command != ERDM_Command.DISCOVERY_COMMAND)
                e.Response = response;
            else if (response != null && response.Command == ERDM_Command.DISCOVERY_COMMAND_RESPONSE)
                RDMSharp.Instance.SendMessage(response);
        }
#endregion
        protected async Task OnReceiveRDMMessage(RDMMessage rdmMessage)
        {
            if ((rdmMessage.DestUID.IsBroadcast || rdmMessage.DestUID == UID) && !rdmMessage.Command.HasFlag(ERDM_Command.RESPONSE))
            {
                if (rdmMessage.SubDevice.IsBroadcast || rdmMessage.SubDevice == this.Subdevice)
                    await RDMSharp.Instance.SendMessage(processRequestMessage(rdmMessage));
            }
        }
        protected sealed override async Task OnResponseMessage(RDMMessage rdmMessage)
        {
            await OnReceiveRDMMessage(rdmMessage);
            await base.OnResponseMessage(rdmMessage);
        }

        protected RDMMessage processRequestMessage(RDMMessage rdmMessage)
        {
            RDMMessage response = null;
            try
            {
                var controllerCache = getControllerCommunicationCache(rdmMessage.SourceUID);
                controllerCache.Seen();
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
                        case ERDM_Parameter.DISC_UNIQUE_BRANCH when rdmMessage.Value is DiscUniqueBranchRequest discUniqueBranchRequest:
                            if (UID >= discUniqueBranchRequest.StartUid && UID <= discUniqueBranchRequest.EndUid)
                            {
                                if (DiscoveryMuted)
                                    return null;
                                response = new RDMMessage
                                {
                                    Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                                    Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                                    SourceUID = UID,
                                    DestUID = rdmMessage.SourceUID,
                                };
                                return response;
                            }
                            return null;
                    }
                }

                if (rdmMessage.SubDevice != SubDevice.Broadcast && !(this.SubDevices?.Any(sd => sd.Subdevice == rdmMessage.SubDevice) ?? true))
                {
                    response = new RDMMessage(ERDM_NackReason.SUB_DEVICE_OUT_OF_RANGE) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                    goto FAIL;
                }
                if (rdmMessage.Command == ERDM_Command.GET_COMMAND)
                {
                    ERDM_Parameter parameter = rdmMessage.Parameter;
                    object requestValue = rdmMessage.Value;
                    byte messageCounter = 0;
                    if (rdmMessage.SubDevice == SubDevice.Broadcast) // no Response on Broadcast Subdevice, because this can't work on a if there are more then one Device responding on a single line.
                    {
                        response = new RDMMessage(ERDM_NackReason.SUB_DEVICE_OUT_OF_RANGE) { Parameter = parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                        goto FAIL;
                    }
                    if (parameter == ERDM_Parameter.QUEUED_MESSAGE)
                    {
                        if (!SupportQueued)
                            goto FAIL;

                        ERDM_Status statusCode = ERDM_Status.NONE;
                        if (requestValue is ERDM_Status status)
                            statusCode = status;

                        if (statusCode == ERDM_Status.NONE)
                        {
                            response = new RDMMessage(ERDM_NackReason.FORMAT_ERROR) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                            goto FAIL;
                        }
                        else if (statusCode == ERDM_Status.GET_LAST_MESSAGE)
                        {
                            response = controllerCache.GetLastSendQueuedOrStatusRDMMessageResponse();
                            if (response != null)
                                goto FAIL;

                            response = new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                            goto FAIL;
                        }

                        if (controllerCache.ParameterUpdatedBag.IsEmpty)
                        {
                            response = new RDMMessage
                            {
                                Parameter = ERDM_Parameter.STATUS_MESSAGES,
                                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                MessageCounter = 0
                            };
                            if (SupportStatus)
                                fillRDMMessageWithStatusMessageData(controllerCache, statusCode, ref response);

                            goto FAIL;
                        }
                        else if (controllerCache.ParameterUpdatedBag.TryDequeue(out var item))
                        {
                            parameter = item.Parameter;
                            requestValue = item.Index;
                            messageCounter = (byte)Math.Min(controllerCache.ParameterUpdatedBag.Count, byte.MaxValue);
                        }
                    }
                    if(parameter == ERDM_Parameter.STATUS_MESSAGES)
                    {
                        ERDM_Status statusCode = ERDM_Status.NONE;
                        if (requestValue is ERDM_Status status)
                            statusCode = status;
                        if (SupportStatus)
                        {
                            if (statusCode == ERDM_Status.GET_LAST_MESSAGE)
                            {
                                response = controllerCache.GetLastSendQueuedOrStatusRDMMessageResponse();
                                if (response != null)
                                    goto FAIL;

                                response = new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                                goto FAIL;
                            }
                            else
                            {
                                response = new RDMMessage
                                {
                                    Parameter = ERDM_Parameter.STATUS_MESSAGES,
                                    Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                    MessageCounter = 0
                                };

                                fillRDMMessageWithStatusMessageData(controllerCache, statusCode, ref response);
                                controllerCache.SetLastSendQueuedOrStatusRDMMessageResponse(response);
                                controllerCache.SetLastSendRDMMessageResponse(response);
                                goto FAIL;
                            }
                        }
                        else goto FAIL;
                    }
                    else
                        removeParameterFromParameterUpdateBag(parameter);

                    parameterValues.TryGetValue(parameter, out object responseValue);

                    if(responseValue is ConcurrentDictionary<object, object> dict)
                    {
                        var val = dict[requestValue];
                        if (val == null)
                            throw new ArgumentOutOfRangeException("No matching id found");
                    }

                    var parameterBag = new ParameterBag(parameter, UID.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                    var define = MetadataFactory.GetDefine(parameterBag);
                    if (define.GetRequest is null)
                    {
                        response = new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                        goto FAIL;
                    }
                    //response = this.Modules.FirstOrDefault(m=>m.IsHandlingParameter(parameter))?.HandleRequest(rdmMessage);
                    if (overflowCacheBags.TryGetValue(rdmMessage.SourceUID, out OverflowCacheBag overflowCache))
                    {
                        if (!overflowCache.Timeouted && overflowCache.Cache.TryDequeue(out byte[] data))
                        {
                            response = new RDMMessage
                            {
                                Parameter = parameter,
                                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                MessageCounter = messageCounter,
                                ParameterData = data,
                            };
                            if (!overflowCache.Cache.TryPeek(out _))
                                overflowCacheBags.Remove(rdmMessage.SourceUID, out _);
                            else
                                response.PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK_OVERFLOW;
                            goto FAIL;
                        }
                    }
                    var dataTreeBranch = DataTreeBranch.FromObject(responseValue, requestValue, parameterBag, ERDM_Command.GET_COMMAND_RESPONSE);
                    try
                    {
                        if (!dataTreeBranch.IsUnset)
                        {
                            var data = MetadataFactory.GetResponseMessageData(parameterBag, dataTreeBranch);
                            if (data != null)
                            {
                                byte[] pData = null;
                                if (data.Count() !=1)
                                {
                                    overflowCacheBags.AddOrUpdate(rdmMessage.SourceUID, (uid) => new OverflowCacheBag(rdmMessage.Parameter, data.ToList()), (uid, o) => new OverflowCacheBag(rdmMessage.Parameter, data.ToList()));
                                    pData = overflowCacheBags[rdmMessage.SourceUID].Cache.Dequeue();
                                }
                                response = new RDMMessage
                                {
                                    Parameter = parameter,
                                    Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                    MessageCounter = messageCounter,
                                    ParameterData = pData ?? data.First(),
                                    PortID_or_Responsetype = pData is null ? (byte)ERDM_ResponseType.ACK : (byte)ERDM_ResponseType.ACK_OVERFLOW
                                };
                            }
                            if (rdmMessage.Parameter == ERDM_Parameter.QUEUED_MESSAGE)
                                controllerCache.SetLastSendQueuedOrStatusRDMMessageResponse(response);
                            controllerCache.SetLastSendRDMMessageResponse(response);
                        }
                        else
                            goto FAIL;
                    }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"ParameterBag:{parameterBag}{Environment.NewLine}Command: {rdmMessage.Command}{Environment.NewLine}RequestValue: {requestValue ?? "<NULL>"}{Environment.NewLine}ResponseValue: {responseValue ?? "<NULL>"}");
                        goto FAIL;
                    }
                }
                else if (rdmMessage.Command == ERDM_Command.SET_COMMAND)
                {
                    bool success = false;
                    //Handle set Request
                    var module = this.Modules.FirstOrDefault(m => m.IsHandlingParameter(rdmMessage.Parameter, rdmMessage.Command));
                    if (module is not null)
                    {
                        response = module.HandleRequest(rdmMessage);
                        if (response != null)
                            goto FAIL;
                    }
                    if (parameterValues.TryGetValue(rdmMessage.Parameter, out object comparisonValue))
                    {
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, (_) =>
                        {
                            try
                            {
                                return rdmMessage.Value;
                            }
                            finally
                            {
                                InvokeParameterValueAdded(new ParameterValueAddedEventArgs(rdmMessage.Parameter, rdmMessage.Value));
                            }
                        }, (_, old) =>
                        {
                            try
                            {
                                return rdmMessage.Value;
                            }
                            finally
                            {
                                InvokeParameterValueChanged(new ParameterValueChangedEventArgs(rdmMessage.Parameter, rdmMessage.Value, old));
                            }
                        });                       
                        success = true;
                        object responseValue = rdmMessage.Value;
                        if (comparisonValue is ConcurrentDictionary<object, object> dict)
                        {
                            switch (rdmMessage.Parameter)
                            {
                                case ERDM_Parameter.SENSOR_VALUE:
                                    if (!object.Equals(rdmMessage.Value, byte.MaxValue))
                                    {
                                        this.Sensors[(byte)rdmMessage.Value].ResetValues();
                                        responseValue = dict[rdmMessage.Value];
                                    }
                                    else //Broadcast
                                    {
                                        foreach (var sensor in this.Sensors.Values)
                                            sensor.ResetValues();
                                        responseValue = new RDMSensorValue(0xff);
                                    }
                                    break;
                                default:
                                    responseValue = dict[rdmMessage.Value];
                                    break;
                            }
                        }

                        try
                        {
                            var parameterBag = new ParameterBag(rdmMessage.Parameter, UID.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                            var dataTreeBranch = DataTreeBranch.FromObject(responseValue, rdmMessage.Value, parameterBag, ERDM_Command.SET_COMMAND_RESPONSE);
                            if (!dataTreeBranch.IsUnset)
                            {
                                var data = MetadataFactory.SetResponseMessageData(parameterBag, dataTreeBranch);
                                if (data != null)
                                    response = new RDMMessage
                                    {
                                        Parameter = rdmMessage.Parameter,
                                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                        ParameterData = data.First(),
                                    };
                            }
                            else
                                goto FAIL;
                        }
                        catch (Exception e)
                        {
                            Logger?.LogError(e);
                            if (e is ArgumentOutOfRangeException || e is KeyNotFoundException)
                                response = new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                            else
                                response = new RDMMessage(ERDM_NackReason.FORMAT_ERROR) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };

                            goto FAIL;
                        }
                    }
                    else if (Parameters.Contains(rdmMessage.Parameter))//Parameter is not in parameterValues
                    {
                        var parameterBag = new ParameterBag(rdmMessage.Parameter, UID.ManufacturerID, DeviceInfo.DeviceModelId, DeviceInfo.SoftwareVersionId);
                        var define = MetadataFactory.GetDefine(parameterBag);
                        switch (rdmMessage.Parameter)
                        {
                            case ERDM_Parameter.RECORD_SENSORS when rdmMessage.Value is byte sensorID:
                                success = true;
                                if (sensorID == 0xFF)//Broadcast
                                    foreach (var sensor in Sensors.Values)
                                        sensor.RecordValue();
                                else
                                    Sensors[sensorID].RecordValue();
                                response = new RDMMessage
                                {
                                    Parameter = rdmMessage.Parameter,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE
                                };
                                goto FAIL;
                            case ERDM_Parameter.CLEAR_STATUS_ID:
                                this.statusMessages.Clear();
                                setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
                                response = new RDMMessage
                                {
                                    Parameter = rdmMessage.Parameter,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE
                                };
                                goto FAIL;

                            default:
                                response = new RDMMessage(ERDM_NackReason.ACTION_NOT_SUPPORTED) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                                goto FAIL;
                        }
                    }
                    else
                        goto FAIL;

                    //Do set Response
                    if (!success)
                    {
                        response = new RDMMessage(ERDM_NackReason.FORMAT_ERROR) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                        goto FAIL;
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(e, string.Empty);

                if (e is ArgumentOutOfRangeException || e is KeyNotFoundException)
                    response = new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                else
                {
                    var define = MetadataFactory.GetDefine(new ParameterBag(rdmMessage.Parameter, (ushort)ManufacturerID, DeviceModelID, SoftwareVersionID));
                    if (
                        (rdmMessage.Command == ERDM_Command.GET_COMMAND && !define.GetRequest.HasValue) ||
                        (rdmMessage.Command == ERDM_Command.SET_COMMAND && !define.SetRequest.HasValue))
                        response = new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                    else
                        response = new RDMMessage(ERDM_NackReason.ACTION_NOT_SUPPORTED) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };
                }
                goto FAIL;
            }
        FAIL:
            if (rdmMessage.SubDevice == SubDevice.Broadcast) // no Response on Broadcast Subdevice, because this can't work on a if there are more then one Device responding on a singel line.
                return null;
            if (rdmMessage.DestUID.IsBroadcast) // no Response on Broadcast
                return null;
            if(response == null)
            {

            }

            response ??= new RDMMessage(ERDM_NackReason.UNKNOWN_PID) { Parameter = rdmMessage.Parameter, Command = rdmMessage.Command | ERDM_Command.RESPONSE };

            response.TransactionCounter = rdmMessage.TransactionCounter;
            response.SourceUID = rdmMessage.DestUID;
            response.DestUID = rdmMessage.SourceUID;
            response.SubDevice = rdmMessage.SubDevice;
            if(response.ResponseType == ERDM_ResponseType.NACK_REASON)
            {
                Logger?.LogTrace($"RDM NACK: {response.NackReason} for Parameter: {response.Parameter} on Device: {this.UID} with Subdevice: {response.SubDevice}");
            }
            return response;
        }
        public sealed override IReadOnlyDictionary<ERDM_Parameter, object> GetAllParameterValues()
        {
            return base.GetAllParameterValues();
        }

        private void updateParameterBag(ERDM_Parameter parameter, object index = null)
        {
            if (!IsInitialized || !_initialized)
                return;
            try
            {
                addOrUpdateParameterFromParameterUpdateBag(parameter, index);
            }
            catch(Exception e)
            {
                Logger?.LogError(e);
            }
        }
        private void addOrUpdateParameterFromParameterUpdateBag(ERDM_Parameter parameter, object index = null)
        {
            foreach (var cache in controllerCommunicationCache)
            {
                if (cache.Value.ParameterUpdatedBag.Any(p => p.Parameter == parameter))
                {
                    var tempQueue = new ConcurrentQueue<ParameterUpdatedBag>();
                    while (cache.Value.ParameterUpdatedBag.TryDequeue(out var item))
                        if (!(item.Parameter.Equals(parameter) && Equals(item.Index, index)))
                            tempQueue.Enqueue(item);


                            while (tempQueue.TryDequeue(out var item))
                                cache.Value.ParameterUpdatedBag.Enqueue(item);
                }
                cache.Value.ParameterUpdatedBag.Enqueue(new ParameterUpdatedBag(parameter, index));
            }
        }
        private void removeParameterFromParameterUpdateBag(ERDM_Parameter parameter, object index = null)
        {
            foreach (var cache in controllerCommunicationCache)
            {
                if (cache.Value.ParameterUpdatedBag.Any(p => p.Parameter == parameter && p.Index == index))
                {
                    var tempQueue = new ConcurrentQueue<ParameterUpdatedBag>();
                    while (cache.Value.ParameterUpdatedBag.TryDequeue(out var item))
                        if (!(item.Parameter.Equals(parameter) && Equals(item.Index, index)))
                            tempQueue.Enqueue(item);


                    while (tempQueue.TryDequeue(out var item))
                        cache.Value.ParameterUpdatedBag.Enqueue(item);
                }
            }
        }

        protected void AddStatusMessage(RDMStatusMessage statusMessage)
        {
            if (!SupportStatus)
                throw new NotSupportedException($"The Device {this.UID} not support Status Messages.");

            int id = 0;
            if (this.statusMessages.Count != 0)
                id = this.statusMessages.Max(s => s.Key) + 1;
            if (this.statusMessages.TryAdd(id, statusMessage))
                setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());

        }
        protected void ClearStatusMessage(RDMStatusMessage statusMessage)
        {
            if (!SupportStatus)
                throw new NotSupportedException($"The Device {this.UID} not support Status Messages.");
            this.statusMessages.Where(s => s.Value.Equals(statusMessage)).ToList().ForEach(s =>
            {
                s.Value.Clear();
            });
            setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
        }
        protected void RemoveStatusMessage(RDMStatusMessage statusMessage)
        {
            if (!SupportStatus)
                throw new NotSupportedException($"The Device {this.UID} not support Status Messages.");

            bool succes = false;
            this.statusMessages.Where(s => s.Value.Equals(statusMessage)).ToList().ForEach(s =>
            {
                if (this.statusMessages.TryRemove(s.Key, out _))
                    succes = true;
            });
            if(succes)
                setParameterValue(ERDM_Parameter.STATUS_MESSAGES, this.statusMessages.Select(sm => sm.Value).ToArray());
        }
        private void fillRDMMessageWithStatusMessageData(ControllerCommunicationCache controllerCache, ERDM_Status statusCode, ref RDMMessage rdmMessage)
        {
            var lastSendStatusMessageID= controllerCache.GetLastSendStatusMessageID(statusCode);
            var _messages = statusMessages.Where(s => s.Key > lastSendStatusMessageID && matchStausCode(statusCode, s.Value)).OrderBy(s => s.Key).ToList();
            if (_messages.Count() != 0)
            {
                byte count = 0;
                List<byte> data = new List<byte>();
                Dictionary<int, RDMStatusMessage> parsedMessages = new Dictionary<int, RDMStatusMessage>();
                while (count < 25 && _messages.Count != 0)
                {
                    var pair = _messages.First();
                    parsedMessages.Add(pair.Key, pair.Value);
                    data.AddRange(pair.Value.ToPayloadData());
                    _messages.RemoveAt(0);
                    count++;
                }
                if (parsedMessages.Count != 0)
                {
                    controllerCache.SetLastSendStatusMessageID(statusCode, _messages.Count == 0 ? -1 : parsedMessages.Max(s => s.Key));
                    if (_messages.Count != 0)
                        rdmMessage.PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK_OVERFLOW;
                    rdmMessage.ParameterData = data.ToArray();
                }
            }
            bool matchStausCode(ERDM_Status statusCode, RDMStatusMessage statusMessage)
            {
                if (statusCode == ERDM_Status.GET_LAST_MESSAGE)
                    throw new NotSupportedException($"The StatusCode: {statusCode}, not supported in this Method.");

                if (statusMessage.EStatusType == ERDM_Status.GET_LAST_MESSAGE)
                    throw new NotSupportedException($"The StatusCode: {statusMessage.EStatusType}, not supported in this Method.");

                var statusType = statusMessage.EStatusType & ~ERDM_Status.CLEARED;

                return statusType >= statusCode;
            }
        }
        private ControllerCommunicationCache getControllerCommunicationCache(UID uid)
        {
            if (!controllerCommunicationCache.TryGetValue(uid, out var cache))
            {
                cache = new ControllerCommunicationCache(uid);
                controllerCommunicationCache.TryAdd(uid, cache);
            }
            return cache;
        }
        protected sealed override void OnDispose()
        {
            RDMSharp.Instance.RequestReceivedEvent -= Instance_RequestReceivedEvent;
            try
            {
                onDispose();
            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }
        }
        protected abstract void onDispose();

        private class OverflowCacheBag
        {
            public readonly ERDM_Parameter Parameter;
            public readonly DateTime CreationTime;
            public readonly Queue<byte[]> Cache = new Queue<byte[]>();

            public bool Timeouted
            {
                get
                {
                    return (DateTime.UtcNow - CreationTime).TotalSeconds > 5;
                }
            }

            public OverflowCacheBag(ERDM_Parameter parameter, IReadOnlyCollection<byte[]> cache)
            {
                Parameter = parameter;
                CreationTime = DateTime.UtcNow;
                foreach (var c in cache)
                    Cache.Enqueue(c);
            }
        }

        private class ControllerCommunicationCache
        {
            public readonly UID Uid;
            public DateTime LastSeen = DateTime.UtcNow;
            private ConcurrentDictionary<ERDM_Status, int> lastSendStatusMessageID;
            private RDMMessage lastSendQueuedOrStatusRDMMessageResponse;
            private RDMMessage lastSendRDMMessageResponse;

            internal ConcurrentQueue<ParameterUpdatedBag> ParameterUpdatedBag = new ConcurrentQueue<ParameterUpdatedBag>();

            public ControllerCommunicationCache(UID uid)
            {
                this.Uid = uid;
            }
            public void Seen()
            {
                LastSeen = DateTime.UtcNow;
            }

            public int GetLastSendStatusMessageID(ERDM_Status statusCode)
            {
                if (lastSendStatusMessageID?.TryGetValue(statusCode, out int id) ?? false)
                    return id;

                return -1;
            }

            public void SetLastSendStatusMessageID(ERDM_Status statusCode, int id)
            {
                if(lastSendStatusMessageID==null)
                    lastSendStatusMessageID = new ConcurrentDictionary<ERDM_Status, int>();

                lastSendStatusMessageID.AddOrUpdate(statusCode, id, (o, p) => id);
            }
            public void SetLastSendQueuedOrStatusRDMMessageResponse(RDMMessage rdmMessage)
            {
                lastSendQueuedOrStatusRDMMessageResponse = rdmMessage;
            }
            public RDMMessage GetLastSendQueuedOrStatusRDMMessageResponse()
            {
                return lastSendQueuedOrStatusRDMMessageResponse;
            }
            public void SetLastSendRDMMessageResponse(RDMMessage rdmMessage)
            {
                lastSendRDMMessageResponse = rdmMessage;
            }
            public RDMMessage GetLastSendRDMMessageResponse()
            {
                return lastSendRDMMessageResponse;
            }
        }
    }
}