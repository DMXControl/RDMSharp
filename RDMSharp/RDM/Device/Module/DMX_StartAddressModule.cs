using System;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class DMX_StartAddressModule : AbstractModule
    {
        private ushort? _dmxAddress;
        public ushort? DMXAddress
        {
            get
            {
                if (ParentDevice is null)
                    return _dmxAddress;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DMX_START_ADDRESS, out res))
                    return (ushort?)res;
                return _dmxAddress;
            }
            set
            {
                if (!value.HasValue)
                    throw new NullReferenceException($"{DMXAddress} can't be null if {ERDM_Parameter.DMX_START_ADDRESS} is Supported");
                if (value.Value == 0)
                    throw new ArgumentOutOfRangeException($"{DMXAddress} can't 0 if {ERDM_Parameter.DMX_START_ADDRESS} is Supported");
                if (value.Value > 512)
                    throw new ArgumentOutOfRangeException($"{DMXAddress} can't be greater then 512");

                _dmxAddress = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.DMX_START_ADDRESS, value);
            }
        }
        public DMX_StartAddressModule(ushort dmxAddress) : base(
            "DMX_StartAddress",
            ERDM_Parameter.DMX_START_ADDRESS)
        {
            _dmxAddress = dmxAddress;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.DMXAddress = _dmxAddress;
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DMX_START_ADDRESS:
                    OnPropertyChanged(nameof(DMXAddress));
                    break;
            }
        }
    }
}