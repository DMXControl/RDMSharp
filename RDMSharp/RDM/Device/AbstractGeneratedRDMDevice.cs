﻿using Microsoft.Extensions.Logging;
using RDMSharp.Metadata;
using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp
{
    public abstract class AbstractGeneratedRDMDevice : AbstractRDMDevice
    {
        public sealed override bool IsGenerated => true;
        #region DeviceInfoStuff
        public readonly ERDM_Parameter[] Parameters;
        public abstract EManufacturer ManufacturerID { get; }
        public abstract ushort DeviceModelID { get; }
        public abstract ERDM_ProductCategoryCoarse ProductCategoryCoarse { get; }
        public abstract ERDM_ProductCategoryFine ProductCategoryFine { get; }
        public abstract uint SoftwareVersionID { get; }
        #endregion
        public abstract string DeviceModelDescription { get; }
        public abstract GeneratedPersonality[] Personalities { get; }
        public new abstract Sensor[] Sensors { get; }

        public abstract bool SupportDMXAddress { get; }

        private RDMDeviceInfo deviceInfo;
        public sealed override RDMDeviceInfo DeviceInfo
        {
            get { return deviceInfo; }
            private protected set
            {
                if (RDMDeviceInfo.Equals(deviceInfo, value))
                    return;

                deviceInfo = value;
                this.OnPropertyChanged(nameof(this.DeviceInfo));
            }
        }
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

        private string deviceLabel;
        public string DeviceLabel
        {
            get
            {
                return deviceLabel;
            }
            set
            {
                if (string.Equals(deviceLabel, value))
                    return;

                deviceLabel = value;
                this.OnPropertyChanged(nameof(this.DeviceLabel));
            }
        }

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

                if (currentPersonality == value)
                    return;

                currentPersonality = value.Value;
                this.OnPropertyChanged(nameof(this.CurrentPersonality));
            }
        }

        protected AbstractGeneratedRDMDevice(UID uid, ERDM_Parameter[] parameters, string manufacturer = null) : base(uid)
        {
            if (!((ushort)ManufacturerID).Equals(uid.ManufacturerID))
                throw new Exception($"{uid.ManufacturerID} not match the {ManufacturerID}");

            #region Parameters
            var _params = parameters.ToList();
            _params.Add(ERDM_Parameter.DEVICE_INFO);
            _params.Add(ERDM_Parameter.SUPPORTED_PARAMETERS);
            _params.Add(ERDM_Parameter.DEVICE_LABEL);
            _params.Add(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION);
            _params.Add(ERDM_Parameter.MANUFACTURER_LABEL);
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
            if ((Sensors?.Length ?? 0) != 0)
            {
                _params.Add(ERDM_Parameter.SENSOR_DEFINITION);
                _params.Add(ERDM_Parameter.SENSOR_VALUE);
                if(Sensors.Any(s=>s.RecordedValueSupported))
                    _params.Add(ERDM_Parameter.RECORD_SENSORS);
            }

            Parameters = _params.Distinct().ToArray();
            trySetParameter(ERDM_Parameter.SUPPORTED_PARAMETERS, Parameters);
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

            #region DeviceLabel
            this.DeviceLabel = this.DeviceModelDescription;
            #endregion

            #region Personalities
            if (Personalities != null)
            {
                if (Personalities.Length >= byte.MaxValue)
                    throw new ArgumentOutOfRangeException($"There to many {Personalities}! Maxumum is {byte.MaxValue - 1}");

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
            if (Sensors != null)
            {
                if (Sensors.Length >= byte.MaxValue)
                    throw new ArgumentOutOfRangeException($"There to many {Sensors}! Maxumum is {byte.MaxValue - 1}");

                if (Sensors.Min(s => s.SensorId) != 0)
                    throw new ArgumentOutOfRangeException($"The first Sensor should have the ID: 0, but is({Sensors.Min(s => s.SensorId)})");
                if (Sensors.Max(s => s.SensorId) + 1 != Sensors.Length)
                    throw new ArgumentOutOfRangeException($"The last Sensor should have the ID: {Sensors.Max(s => s.SensorId) + 1}, but is({Sensors.Max(s => s.SensorId)})");

                if (Sensors.Select(s=>s.SensorId).Distinct().Count() != Sensors.Length)
                    throw new ArgumentOutOfRangeException($"Some Sensor-IDs are used more then onse");


                if (Sensors.Length != 0)
                {
                    var sensorDef = new ConcurrentDictionary<object, object>();
                    var sensorValue = new ConcurrentDictionary<object, object>();
                    foreach (var sensor in Sensors)
                    {
                        if (!sensorDef.TryAdd(sensor.SensorId, (RDMSensorDefinition)sensor))
                            throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorDefinition)}");

                        if (!sensorValue.TryAdd(sensor.SensorId, (RDMSensorValue)sensor))
                            throw new Exception($"{sensor.SensorId} already used as {nameof(RDMSensorValue)}");

                        SetGeneratedSensorDescription((RDMSensorDefinition)sensor);
                        SetGeneratedSensorValue((RDMSensorValue)sensor);
                        sensor.PropertyChanged += (o, e) =>
                        {
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
                                    SetGeneratedSensorDescription((RDMSensorDefinition)sensor);
                                    break;
                                case nameof(Sensor.PresentValue):
                                case nameof(Sensor.LowestValue):
                                case nameof(Sensor.HighestValue):
                                case nameof(Sensor.RecordedValue):
                                    SetGeneratedSensorValue((RDMSensorValue)sensor);
                                    sensorValue.AddOrUpdate(sensor.SensorId, (RDMSensorValue)sensor, (o1, o2) => (RDMSensorValue)sensor);
                                    SetGeneratedParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);
                                    break;
                            }
                        };
                    }

                    SetGeneratedParameterValue(ERDM_Parameter.SENSOR_DEFINITION, sensorDef);
                    SetGeneratedParameterValue(ERDM_Parameter.SENSOR_VALUE, sensorValue);
                }
            }
            #endregion

            #region DMX-Address
            if (Parameters.Contains(ERDM_Parameter.DMX_START_ADDRESS))
                DMXAddress = 1;
            #endregion

            updateDeviceInfo();
        }
        private void updateDeviceInfo()
        {
            DeviceInfo = new RDMDeviceInfo(1,
                                           0,
                                           DeviceModelID,
                                           ProductCategoryCoarse,
                                           ProductCategoryFine,
                                           SoftwareVersionID,
                                           dmx512Footprint: Personalities.FirstOrDefault(p => p.ID == currentPersonality)?.SlotCount ?? 0,
                                           dmx512CurrentPersonality: currentPersonality,
                                           dmx512NumberOfPersonalities: (byte)(Personalities?.Length ?? 0),
                                           dmx512StartAddress: dmxAddress,
                                           sensorCount: (byte)(Sensors?.Length ?? 0));
        }
        protected bool trySetParameter(ERDM_Parameter parameter, object value)
        {
            if (!this.Parameters.Contains(parameter))
                throw new NotSupportedException($"The Parameter: {parameter}, is not Supported");

           
            base.SetGeneratedParameterValue(parameter, value);
            return true;
        }
        public bool TrySetParameter(ERDM_Parameter parameter, object value, bool throwException = true)
        {

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
                try
                {
                    if (!define.SetRequest.HasValue)
                        throw new NotSupportedException($"The Protocoll not allow to set the Parameter: {parameter}");
                    else
                    {
                        byte[] data = MetadataFactory.ParsePayloadToData(define, Metadata.JSON.Command.ECommandDublicte.SetRequest, DataTreeBranch.FromObject(value, null, parameterBag, ERDM_Command.SET_COMMAND));
                        var obj = MetadataFactory.ParseDataToPayload(define, Metadata.JSON.Command.ECommandDublicte.SetRequest, data);
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
                    trySetParameter(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, this.DeviceInfo.SoftwareVersionId);
                    break;
                case nameof(DeviceModelDescription):
                    trySetParameter(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, this.DeviceModelDescription);
                    break;
                case nameof(DMXAddress):
                    trySetParameter(ERDM_Parameter.DMX_START_ADDRESS, this.DMXAddress);
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
                case nameof(DeviceLabel):
                    trySetParameter(ERDM_Parameter.DEVICE_LABEL, this.DeviceLabel);
                    break;
                case nameof(ManufacturerLabel):
                    trySetParameter(ERDM_Parameter.MANUFACTURER_LABEL, this.ManufacturerLabel);
                    break;
            }
            base.OnPropertyChanged(property);
        }
        protected override bool OnUpdateParametrerValueCache(ERDM_Parameter parameter, object value)
        {
            try
            {
                switch (parameter)
                {
                    case ERDM_Parameter.DMX_START_ADDRESS
                    when value is ushort dmxAddr:
                        if (dmxAddr >= 1 && dmxAddr <= 512)
                            DMXAddress = dmxAddr;
                        else
                            return false;
                        break;
                    case ERDM_Parameter.DMX_PERSONALITY
                    when value is byte pers:
                        if (pers >= 1 && pers <= byte.MaxValue)
                            CurrentPersonality = pers;
                        else
                            return false;
                        break;
                    case ERDM_Parameter.DEVICE_LABEL
                    when value is string devLabel:
                        DeviceLabel = devLabel;
                        break;
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(e, string.Empty);
                return false;
            }
            return true;
        }
    }
}