using org.dmxc.wkdt.Light.RDM;
using System;
using System.Security.Cryptography;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class ManufacturerLabelModule : AbstractModule
    {
        private string _manufacturerLabel;
        public string ManufacturerLabel
        {
            get
            {
                if (ParentDevice is null)
                    return _manufacturerLabel;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.MANUFACTURER_LABEL, out res))
                    return (string)res;
                return _manufacturerLabel;
            }
            internal set
            {
                _manufacturerLabel = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.MANUFACTURER_LABEL, value);
            }
        }
        public ManufacturerLabelModule(string manufacturerLabel) : base(
            "ManufacturerLabel",
            ERDM_Parameter.MANUFACTURER_LABEL)
        {
            _manufacturerLabel = manufacturerLabel;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            if (string.IsNullOrWhiteSpace(_manufacturerLabel))
                _manufacturerLabel = Enum.GetName(typeof(EManufacturer), (EManufacturer)device.UID.ManufacturerID);
            this.ManufacturerLabel = _manufacturerLabel;
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.MANUFACTURER_LABEL:
                    OnPropertyChanged(nameof(ManufacturerLabel));
                    break;
            }
        }
    }
}