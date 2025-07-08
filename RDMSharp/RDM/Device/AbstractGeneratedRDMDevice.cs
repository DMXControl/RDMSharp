using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        public virtual bool SupportRealTimeClock { get; }
        #region DeviceInfoStuff
        private HashSet<ERDM_Parameter> _parameters;
        public IReadOnlySet<ERDM_Parameter> Parameters { get => _parameters; }
        public abstract EManufacturer ManufacturerID { get; }
        public abstract ushort DeviceModelID { get; }
        public abstract ERDM_ProductCategoryCoarse ProductCategoryCoarse { get; }
        public abstract ERDM_ProductCategoryFine ProductCategoryFine { get; }
        public abstract uint SoftwareVersionID { get; }
        #endregion
        public abstract string DeviceModelDescription { get; }
        public abstract GeneratedPersonality[] Personalities { get; }

        private readonly ConcurrentDictionary<byte, Sensor> sensors = new ConcurrentDictionary<byte, Sensor>();
        public sealed override IReadOnlyDictionary<byte, Sensor> Sensors { get { return sensors.AsReadOnly(); } }
        private ConcurrentDictionary<object, object> sensorDef;
        private ConcurrentDictionary<object, object> sensorValue;

        public sealed override IReadOnlyDictionary<ushort, Slot> Slots { get { return CurrentPersonality.HasValue ? Personalities?.FirstOrDefault(p => p.ID.Equals(CurrentPersonality.Value))?.Slots : null; } }

        private ConcurrentDictionary<int, RDMStatusMessage> statusMessages = new ConcurrentDictionary<int, RDMStatusMessage>();
        public sealed override IReadOnlyDictionary<int, RDMStatusMessage> StatusMessages { get { return statusMessages.AsReadOnly(); } }
        private ConcurrentDictionary<UID, ControllerCommunicationCache> controllerCommunicationCache = new ConcurrentDictionary<UID, ControllerCommunicationCache>();
        public abstract bool SupportDMXAddress { get; }

        private RDMDeviceInfo deviceInfo;
        public sealed override RDMDeviceInfo DeviceInfo { get { return deviceInfo; } }

        private ConcurrentDictionary<UID, OverflowCacheBag> overflowCacheBags = new ConcurrentDictionary<UID, OverflowCacheBag>();

        private readonly IReadOnlyCollection<IModule> _modules;
        public IReadOnlyCollection<IModule> Modules { get => _modules; }

        private ushort dmxAddress { get; set; }
        public ushort? DMXAddress
        {
            get
            {
                if (!this.Parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
                    return null;

                return dmxAddress;
            }
            set
            {
                if (!this.Parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
                {
                    dmxAddress = 0;
                    return;
                }
                if (!value.HasValue)
                    throw new NullReferenceException($"{DMXAddress} can't be null if {ERDM_Parameter.DMX_START_ADDRESS} is Supported");
                if (value.Value == 0)
                    throw new ArgumentOutOfRangeException($"{DMXAddress} can't 0 if {ERDM_Parameter.DMX_START_ADDRESS} is Supported");

                if (dmxAddress == value.Value)
                    return;

                dmxAddress = value.Value;
                this.OnPropertyChanged(nameof(this.DMXAddress));
                this.updateDeviceInfo();
            }
        }
        public readonly string ManufacturerLabel;

        private byte currentPersonality;
        public byte? CurrentPersonality
        {
            get
            {
                if (!this.Parameters.Contains(ERDM_Parameter.DMX_PERSONALITY))
                    return null;

                return currentPersonality;
            }
            set
            {
                if (!this.Parameters.Contains(ERDM_Parameter.DMX_PERSONALITY))
                {
                    currentPersonality = 0;
                    return;
                }
                if (!value.HasValue)
                    throw new NullReferenceException($"{CurrentPersonality} can't be null if {ERDM_Parameter.DMX_PERSONALITY} is Supported");
                if (value.Value == 0)
                    throw new ArgumentOutOfRangeException($"{CurrentPersonality} can't 0 if {ERDM_Parameter.DMX_PERSONALITY} is Supported");

                if (!this.Personalities.Any(p => p.ID == value.Value))
                    throw new ArgumentOutOfRangeException($"No Personality found with ID: {value.Value}");

                if (currentPersonality == value)
                    return;

                currentPersonality = value.Value;
                this.OnPropertyChanged(nameof(this.CurrentPersonality));
                this.updateDeviceInfo();
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

        private bool identify;
        public bool Identify
        {
            get
            {
                return identify;
            }
            set
            {
                if (string.Equals(identify, value))
                    return;

                identify = value;
                this.OnPropertyChanged(nameof(this.Identify));
            }
        }


        private string softwareVersionLabel;
        public string SoftwareVersionLabel
        {
            get
            {
                return softwareVersionLabel;
            }
            protected set
            {
                if (string.Equals(softwareVersionLabel, value))
                    return;

                softwareVersionLabel = value;
                this.OnPropertyChanged(nameof(this.SoftwareVersionLabel));
            }
        }

        private bool _initialized = false;

        protected AbstractGeneratedRDMDevice(UID uid, ERDM_Parameter[] parameters, string manufacturer = null, Sensor[] sensors = null, IRDMDevice[] subDevices = null, IReadOnlyCollection<IModule> modules = null) : this(uid, SubDevice.Root, parameters, manufacturer, sensors, subDevices, modules)
        {
        }
        protected AbstractGeneratedRDMDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, string manufacturer = null, Sensor[] sensors = null, IReadOnlyCollection<IModule> modules =null) : this(uid, subDevice, parameters, manufacturer, sensors, null, modules)
        {
        }
        private AbstractGeneratedRDMDevice(UID uid, SubDevice subDevice, ERDM_Parameter[] parameters, string manufacturer = null, Sensor[] sensors = null, IRDMDevice[] subDevices = null, IReadOnlyCollection<IModule> modules=null) : base(uid, subDevice, subDevices)
        {
            if (!((ushort)ManufacturerID).Equals(uid.ManufacturerID))
                throw new Exception($"{uid.ManufacturerID} not match the {ManufacturerID}");

            RDMSharp.Instance.RequestReceivedEvent += Instance_RequestReceivedEvent;

            if (modules is not null)
                _modules = modules;


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
            _params.Add(ERDM_Parameter.SOFTWARE_VERSION_LABEL);
            _params.Add(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID);
            _params.Add(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL);
            _params.Add(ERDM_Parameter.DEVICE_LABEL);
            _params.Add(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION);
            _params.Add(ERDM_Parameter.MANUFACTURER_LABEL);
            _params.Add(ERDM_Parameter.IDENTIFY_DEVICE);
            if (SupportRealTimeClock)
                _params.Add(ERDM_Parameter.REAL_TIME_CLOCK);
            if (SupportDMXAddress)
                _params.Add(ERDM_Parameter.DMX_START_ADDRESS);
            if ((Personalities?.Length ?? 0) != 0)
            {
                _params.Add(ERDM_Parameter.DMX_PERSONALITY);
                _params.Add(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION);
                _params.Add(ERDM_Parameter.SLOT_INFO);
                _params.Add(ERDM_Parameter.SLOT_DESCRIPTION);
                _params.Add(ERDM_Parameter.DEFAULT_SLOT_VALUE);
            }

            if (modules is not null)
                foreach (IModule module in modules)
                    foreach (var parameter in module.SupportedParameters)
                        _params.Add(parameter);

                _parameters = _params;

            if (modules is not null)
                foreach (IModule module in modules)
                    if (module is AbstractModule aModule)
                        aModule.SetParentDevice(this);

            if (SupportRealTimeClock)
                trySetParameter(ERDM_Parameter.REAL_TIME_CLOCK, new RDMRealTimeClock(DateTime.Now));
            trySetParameter(ERDM_Parameter.SUPPORTED_PARAMETERS, Parameters.ToArray());
            trySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, Identify);


            #endregion

            #region ManufacturerLabel
            string _manufacturer = Enum.GetName(typeof(EManufacturer), (EManufacturer)uid.ManufacturerID);

            if (string.IsNullOrWhiteSpace(_manufacturer))
                _manufacturer = manufacturer;
            if (string.IsNullOrWhiteSpace(_manufacturer))
                throw new ArgumentNullException($"{manufacturer} not set, needed in case the Manufacturer is not Part of {typeof(EManufacturer).Name}");

            ManufacturerLabel = _manufacturer;
            this.OnPropertyChanged(nameof(this.ManufacturerLabel));
            #endregion

            #region DeviceModelDescription
            this.OnPropertyChanged(nameof(this.DeviceModelDescription));
            #endregion

            #region Personalities
            if (Personalities != null)
            {
                if (Personalities.Length >= byte.MaxValue)
                    throw new ArgumentOutOfRangeException($"There to many {Personalities}! Maximum is {byte.MaxValue - 1}");

                if (Personalities.Length != 0)
                {
                    var persDesc = new ConcurrentDictionary<object, object>();
                    foreach (var gPers in Personalities)
                        if (!persDesc.TryAdd(gPers.ID, (RDMDMXPersonalityDescription)gPers))
                            throw new Exception($"{gPers.ID} already used as {nameof(gPers.ID)}");

                    trySetParameter(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, persDesc);
                }
                CurrentPersonality = 1;
            }
            #endregion

            #region Sensors

            if (sensors != null)
                this.AddSensors(sensors);

            #endregion

            #region StatusMessage
            if (SupportStatus)
                trySetParameter(ERDM_Parameter.STATUS_MESSAGES, new RDMStatusMessage[0]);
            #endregion

            #region DMX-Address
            if (Parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
                DMXAddress = 1;
            #endregion

            updateDeviceInfo();
            _initialized = true;
        }

        private void updateDeviceInfo()
        {
            var info = new RDMDeviceInfo(1,
                                           0,
                                           DeviceModelID,
                                           ProductCategoryCoarse,
                                           ProductCategoryFine,
                                           SoftwareVersionID,
                                           dmx512Footprint: Personalities.FirstOrDefault(p => p.ID == currentPersonality)?.SlotCount ?? 0,
                                           dmx512CurrentPersonality: currentPersonality,
                                           dmx512NumberOfPersonalities: (byte)(Personalities?.Length ?? 0),
                                           dmx512StartAddress: dmxAddress,
                                           subDeviceCount: (ushort)(SubDevices?.Where(sd=>!sd.Subdevice.IsRoot).Count() ?? 0),
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

        protected void AddSensors(params Sensor[] @sensors)
        {
            if (sensors == null)
                throw new ArgumentNullException();

            if (sensorDef is null)
                sensorDef = new ConcurrentDictionary<object, object>();
            if (sensorValue is null)
                sensorValue = new ConcurrentDictionary<object, object>();
            foreach (var sensor in @sensors)
            {
                if (sensor == null)
                    throw new ArgumentNullException(nameof(sensor));
                if (this.sensors.ContainsKey(sensor.SensorId))
                    throw new ArgumentOutOfRangeException($"The Sensor with the ID: {sensor.SensorId} already exists");

                if (this.sensors.TryAdd(sensor.SensorId, sensor))
                {
                    if (!sensorDef.TryAdd(sensor.SensorId, (RDMSensorDefinition)sensor))
                        throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorDefinition)}");

                    if (!sensorValue.TryAdd(sensor.SensorId, (RDMSensorValue)sensor))
                        throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorValue)}");

                    sensor.PropertyChanged += Sensor_PropertyChanged;
                }
            }

            var _sensors = Sensors.Values.ToArray();
            if (_sensors.Length >= byte.MaxValue)
                throw new ArgumentOutOfRangeException($"There to many {Sensors}! Maximum is {byte.MaxValue - 1}");
            if (_sensors.Length > 0)
            {
                if (_sensors.Min(s => s.SensorId) != 0)
                    throw new ArgumentOutOfRangeException($"The first Sensor should have the ID: 0, but is({_sensors.Min(s => s.SensorId)})");
                if (_sensors.Max(s => s.SensorId) + 1 != _sensors.Length)
                    throw new ArgumentOutOfRangeException($"The last Sensor should have the ID: {_sensors.Max(s => s.SensorId) + 1}, but is({_sensors.Max(s => s.SensorId)})");

                if (_sensors.Select(s => s.SensorId).Distinct().Count() != _sensors.Length)
                    throw new ArgumentOutOfRangeException($"Some Sensor-IDs are used more then onse");

                setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef);
                setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);
            }

            updateSupportedParametersOnAddRemoveSensors();
            updateDeviceInfo();
        }
        protected void RemoveSensors(params Sensor[] @sensors)
        {
            foreach (var sensor in @sensors)
            {
                if (sensor == null)
                    throw new ArgumentNullException(nameof(sensor));
                if (!this.sensors.ContainsKey(sensor.SensorId))
                    throw new ArgumentOutOfRangeException($"The Sensor with the ID: {sensor.SensorId} not exists");
                if (this.sensors.TryRemove(sensor.SensorId, out _))
                {
                    sensor.PropertyChanged -= Sensor_PropertyChanged;
                    if (parameterValues.TryGetValue(ERDM_Parameter.SENSOR_DEFINITION, out object value_d) && value_d is ConcurrentDictionary<object, object> sensorDef)
                    {
                        if (sensorDef.TryRemove(sensor.SensorId, out _))
                            setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef);
                    }
                    if (parameterValues.TryGetValue(ERDM_Parameter.SENSOR_VALUE, out object value_v) && value_v is ConcurrentDictionary<object, object> sensorValue)
                    {
                        if (sensorValue.TryRemove(sensor.SensorId, out _))
                            setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);
                    }
                }
            }

            if (sensorDef?.IsEmpty ?? false)
                sensorDef = null;
            if (sensorValue?.IsEmpty ?? false)
                sensorValue = null;

            setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef);
            setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);

            updateSupportedParametersOnAddRemoveSensors();
            updateDeviceInfo();
        }

        private void updateSupportedParametersOnAddRemoveSensors()
        {
            var oldParameters = Parameters;
            if (!this.sensors.IsEmpty)
            {
                HashSet<ERDM_Parameter> _params = Parameters.ToHashSet();
                _params.Add(ERDM_Parameter.SENSOR_DEFINITION);
                _params.Add(ERDM_Parameter.SENSOR_VALUE);
                _parameters = _params;
            }
            else if (
                Parameters.Contains(ERDM_Parameter.SENSOR_DEFINITION) ||
                Parameters.Contains(ERDM_Parameter.SENSOR_VALUE) ||
                Parameters.Contains(ERDM_Parameter.RECORD_SENSORS) ||
                Parameters.Contains(ERDM_Parameter.SENSOR_TYPE_CUSTOM) ||
                Parameters.Contains(ERDM_Parameter.SENSOR_UNIT_CUSTOM))
            {
                HashSet<ERDM_Parameter> _params = Parameters.ToHashSet();
                _params.RemoveWhere(p =>
                    p == ERDM_Parameter.SENSOR_DEFINITION ||
                    p == ERDM_Parameter.SENSOR_VALUE ||
                    p == ERDM_Parameter.RECORD_SENSORS ||
                    p == ERDM_Parameter.SENSOR_TYPE_CUSTOM ||
                    p == ERDM_Parameter.SENSOR_UNIT_CUSTOM);
                _parameters = _params;
            }

            if (sensors.Values.Any(s => s.RecordedValueSupported) && !Parameters.Contains(ERDM_Parameter.RECORD_SENSORS))
            {
                HashSet<ERDM_Parameter> _params = Parameters.ToHashSet();
                _params.Add(ERDM_Parameter.RECORD_SENSORS);
                _parameters = _params;
            }
            else if (!sensors.Values.Any(s => s.RecordedValueSupported) && Parameters.Contains(ERDM_Parameter.RECORD_SENSORS))
            {
                HashSet<ERDM_Parameter> _params = Parameters.ToHashSet();
                _params.RemoveWhere(sensors => sensors == ERDM_Parameter.RECORD_SENSORS);
                _parameters = _params;
            }

            if (!Parameters.SequenceEqual(oldParameters))
                trySetParameter(ERDM_Parameter.SUPPORTED_PARAMETERS, Parameters.ToArray());
        }

        private void Sensor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not Sensor sensor)
                return;

            switch (e.PropertyName)
            {
                case nameof(Sensor.Type):
                case nameof(Sensor.Unit):
                case nameof(Sensor.Prefix):
                case nameof(Sensor.RangeMaximum):
                case nameof(Sensor.RangeMinimum):
                case nameof(Sensor.NormalMaximum):
                case nameof(Sensor.NormalMinimum):
                case nameof(Sensor.LowestHighestValueSupported):
                case nameof(Sensor.RecordedValueSupported):
                    sensorDef.AddOrUpdate(sensor.SensorId, (RDMSensorDefinition)sensor, (o1, o2) => (RDMSensorDefinition)sensor);
                    setParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef, sensor.SensorId);
                    break;
                case nameof(Sensor.PresentValue):
                case nameof(Sensor.LowestValue):
                case nameof(Sensor.HighestValue):
                case nameof(Sensor.RecordedValue):
                    sensorValue.AddOrUpdate(sensor.SensorId, (RDMSensorValue)sensor, (o1, o2) => (RDMSensorValue)sensor);
                    setParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue, sensor.SensorId);
                    break;
            }
        }

        internal protected bool trySetParameter(ERDM_Parameter parameter, object value)
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
        protected sealed override void OnPropertyChanged(string property)
        {
            switch (property)
            {
                case nameof(DeviceInfo):
                    trySetParameter(ERDM_Parameter.DEVICE_INFO, this.DeviceInfo);
                    trySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, this.DeviceInfo.SoftwareVersionId);
                    break;
                case nameof(DeviceModelDescription):
                    trySetParameter(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, this.DeviceModelDescription);
                    break;
                case nameof(DMXAddress):
                    trySetParameter(ERDM_Parameter.DMX_START_ADDRESS, this.DMXAddress);
                    break;
                case nameof(Identify):
                    trySetParameter(ERDM_Parameter.IDENTIFY_DEVICE, this.Identify);
                    break;
                case nameof(CurrentPersonality):
                    trySetParameter(ERDM_Parameter.DMX_PERSONALITY, new RDMDMXPersonality(this.currentPersonality, (byte)(Personalities?.Length ?? 0)));

                    var slots = Personalities.First(p => p.ID == this.currentPersonality).Slots.Count;
                    var slotInfos = new RDMSlotInfo[slots];
                    var slotDesc = new ConcurrentDictionary<object, object>();
                    var slotDefault = new RDMDefaultSlotValue[slots];
                    foreach (var s in Personalities.First(p => p.ID == this.currentPersonality).Slots)
                    {
                        Slot slot = s.Value;
                        slotInfos[slot.SlotId] = new RDMSlotInfo(slot.SlotId, slot.Type, slot.Category);
                        slotDesc.TryAdd(slot.SlotId, new RDMSlotDescription(slot.SlotId, slot.Description));
                        slotDefault[slot.SlotId] = new RDMDefaultSlotValue(slot.SlotId, slot.DefaultValue);
                    }
                    trySetParameter(ERDM_Parameter.SLOT_INFO, slotInfos);
                    trySetParameter(ERDM_Parameter.SLOT_DESCRIPTION, slotDesc);
                    trySetParameter(ERDM_Parameter.DEFAULT_SLOT_VALUE, slotDefault);
                    break;
                case nameof(ManufacturerLabel):
                    trySetParameter(ERDM_Parameter.MANUFACTURER_LABEL, this.ManufacturerLabel);
                    break;
                case nameof(SoftwareVersionLabel):
                    trySetParameter(ERDM_Parameter.SOFTWARE_VERSION_LABEL, this.SoftwareVersionLabel);
                    break;
            }
            base.OnPropertyChanged(property);
        }


        public bool SetParameter(ERDM_Parameter parameter, object value = null)
        {
            setParameterValue(parameter, value);
            return true;
        }
        private void setParameterValue(ERDM_Parameter parameter, object value, object index=null)
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
                    parameterValues.AddOrUpdate(parameter, value, (o, p) =>
                    {
                        if (object.Equals(p, value)&& value is not ConcurrentDictionary<object, object>)
                            notNew = true;
                        return value;
                    });
                    if (notNew)
                        return;
                    if (parameter != ERDM_Parameter.SLOT_DESCRIPTION)
                    {
                        updateParameterBag(parameter, index);
                        return;
                    }
                    if(value is ConcurrentDictionary<object, object> dict)
                    {
                        foreach (var p in dict)
                            updateParameterBag(parameter, p.Key);
                    }
                    return;
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
                    if (parameterValues.TryGetValue(rdmMessage.Parameter, out object comparisonValue))
                    {
                        parameterValues.AddOrUpdate(rdmMessage.Parameter, (_) => rdmMessage.Value, (_,_) => rdmMessage.Value);                        
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
                            updateParameterFromRemote(rdmMessage.Parameter, responseValue);
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

        private void updateParameterFromRemote(ERDM_Parameter parameter, object value)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DMX_START_ADDRESS:
                    DMXAddress = (ushort)value;
                    break;
                case ERDM_Parameter.DMX_PERSONALITY:
                    CurrentPersonality = (byte)value;
                    break;
                case ERDM_Parameter.IDENTIFY_DEVICE:
                    Identify = (bool)value;
                    break;
            }
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