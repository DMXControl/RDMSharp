namespace RDMSharp.RDM.Device.Module
{
    public sealed class DeviceModelDescriptionModule : AbstractModule
    {
        private string _deviceModelDescriptionLabel;
        public string DeviceModelDescription
        {
            get
            {
                if (ParentDevice is null)
                    return _deviceModelDescriptionLabel;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, out res))
                    return (string)res;
                return _deviceModelDescriptionLabel;
            }
            internal set
            {
                _deviceModelDescriptionLabel = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, value);
            }
        }
        public DeviceModelDescriptionModule(string manufacturerLabel) : base(
            "DeviceModelDescription",
            ERDM_Parameter.DEVICE_MODEL_DESCRIPTION)
        {
            _deviceModelDescriptionLabel = manufacturerLabel;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.DeviceModelDescription = _deviceModelDescriptionLabel;
        }
        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.DEVICE_MODEL_DESCRIPTION:
                    OnPropertyChanged(nameof(DeviceModelDescription));
                    break;
            }
        }
    }
}