namespace RDMSharp.RDM.Device.Module
{
    public sealed class BootSoftwareVersionModule : AbstractModule
    {
        private uint _bootSoftwareVersionId;
        private string _bootSoftwareVersionLabel;
        public uint BootSoftwareVersionId
        {
            get
            {
                if (ParentDevice is null)
                    return _bootSoftwareVersionId;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, out res))
                    return (uint)res;
                return _bootSoftwareVersionId;
            }
            internal set {
                _bootSoftwareVersionId = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, value);
            }
        }
        public string BootSoftwareVersionLabel
        {
            get
            {
                if (ParentDevice is null)
                    return _bootSoftwareVersionLabel;
                object res;
                if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, out res))
                    return (string)res;
                return _bootSoftwareVersionLabel;
            }
            internal set
            {
                _bootSoftwareVersionLabel = value;
                if (ParentDevice is not null)
                    ParentDevice.setParameterValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, value);
            }
        }
        public BootSoftwareVersionModule(uint bootSoftwareVersionId, string bootSoftwareVersionLabel) : base(
            "BootSoftwareVersion",
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
            ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL)
        {
            _bootSoftwareVersionId = bootSoftwareVersionId;
            _bootSoftwareVersionLabel = bootSoftwareVersionLabel;
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.BootSoftwareVersionId = _bootSoftwareVersionId;
            this.BootSoftwareVersionLabel = _bootSoftwareVersionLabel;
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID:
                    OnPropertyChanged(nameof(BootSoftwareVersionId));
                    break;
                case ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL:
                    OnPropertyChanged(nameof(BootSoftwareVersionLabel));
                    break;
            }
        }
    }
}