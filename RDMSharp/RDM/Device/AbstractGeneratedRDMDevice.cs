using RDMSharp.ParameterWrapper;
using System;
using System.Collections.Concurrent;
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
        public abstract GeneratedPersonality[] Personalities {get;}

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

        protected AbstractGeneratedRDMDevice(RDMUID uid, ERDM_Parameter[] parameters, string manufacturer = null) : base(uid)
        {
            if (!((ushort)ManufacturerID).Equals(uid.ManufacturerID))
                throw new Exception($"{uid.ManufacturerID} not match the {ManufacturerID}");

            #region Parameters
            var _params = parameters.ToList();
            _params.Add(ERDM_Parameter.DEVICE_INFO);
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

            Parameters = _params.Distinct().ToArray();
            #endregion

            #region ManufacturerLabel
            string _manufacturer = Enum.GetName(typeof(EManufacturer), uid.Manufacturer);

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
                                           dmx512CurrentPersonality: currentPersonality,
                                           dmx512NumberOfPersonalities: (byte)(Personalities?.Length ?? 0),
                                           dmx512StartAddress: dmxAddress);
        }
        private bool trySetParameter(ERDM_Parameter parameter, object value)
        {
            if (!this.Parameters.Contains(parameter))
                throw new NotSupportedException($"The Parameter: {parameter}, is not Supported");

            switch (pmManager.GetRDMParameterWrapperByID(parameter))
            {
                case IRDMSetParameterWrapperResponse setParameterWrapperResponse:
                    if (setParameterWrapperResponse.SetResponseType != value.GetType())
                        throw new NotSupportedException($"The Type of {nameof(value)} is not supported as Response for Parameter: {parameter}, the only supported Type is {setParameterWrapperResponse.SetResponseType}");
                    break;
            }
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
            switch (pmManager.GetRDMParameterWrapperByID(parameter))
            {
                case IRDMDescriptionParameterWrapper descriptionParameterWrapper:
                    if (throwException)
                        throw new NotSupportedException($"You have no permission to set the Parameter: {parameter}, use the public Propertys to set them");
                    return false;
            }

            return this.trySetParameter(parameter, value);
        }
        protected sealed override void OnPropertyChanged(string property)
        {
            switch (property)
            {
                case nameof(DeviceInfo):
                    trySetParameter(ERDM_Parameter.DEVICE_INFO, this.DeviceInfo);
                    break;
                case nameof(DeviceModelDescription):
                    trySetParameter(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, this.DeviceModelDescription);
                    break;
                case nameof(DMXAddress):
                    trySetParameter(ERDM_Parameter.DMX_START_ADDRESS, this.DMXAddress);
                    break;
                case nameof(CurrentPersonality):
                    trySetParameter(ERDM_Parameter.DMX_PERSONALITY, new RDMDMXPersonality(this.currentPersonality, (byte)(Personalities?.Length ?? 0)));

                    var slotInfos = new ConcurrentDictionary<object, object>();
                    var slotDesc = new ConcurrentDictionary<object, object>();
                    var slotDefault = new ConcurrentDictionary<object, object>();
                    foreach (var s in Personalities.First(p => p.ID == this.currentPersonality).Slots) 
                    {
                        Slot slot = s.Value;
                        slotInfos.TryAdd(slot.SlotId, new RDMSlotInfo(slot.SlotId, slot.Type, slot.Category));
                        slotDesc.TryAdd(slot.SlotId, new RDMSlotDescription(slot.SlotId,slot.Description));
                        slotDefault.TryAdd(slot.SlotId, new RDMDefaultSlotValue(slot.SlotId, slot.DefaultValue));
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
                return false;
            }
            return true;
        }
    }
}