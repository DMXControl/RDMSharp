namespace RDMSharp.RDM.Device.Module
{
    public sealed class DeviceLabelModule : AbstractModule
    {
        private string _deviceLabel;
        public string DeviceLabel
        {
            get
            {
                if (ParentDevice is null)
                    return _deviceLabel;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_LABEL, out res))
                    return (string)res;
                return _deviceLabel;
            }
            internal set
            {
                _deviceLabel = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.DEVICE_LABEL, value);
            }
        }
        public DeviceLabelModule(string deviceLabel) : base(
            "DeviceLabel",
            ERDM_Parameter.DEVICE_LABEL)
        {
            _deviceLabel = deviceLabel;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.DeviceLabel = _deviceLabel;
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_LABEL:
                    OnPropertyChanged(nameof(DeviceLabel));
                    break;
            }
        }
    }
}