namespace RDMSharp.RDM.Device.Module;

public sealed class BootSoftwareVersionModule : AbstractModule
{
    private const string _moduleName = "BootSoftwareVersion";
    private const string _moduleDisplayName = "Boot Software Version";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
        ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL
    };

    public override string DisplayName => _moduleDisplayName;

    private uint? _bootSoftwareVersionId;
    private string _bootSoftwareVersionLabel;
    public uint? BootSoftwareVersionId
    {
        get
        {
            if (ParentDevice is null)
                return _bootSoftwareVersionId;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, out object res))
                return (uint)res;
            return _bootSoftwareVersionId;
        }
        internal set
        {
            _bootSoftwareVersionId = value;
            ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID, value);
        }
    }
    public string BootSoftwareVersionLabel
    {
        get
        {
            if (ParentDevice is null)
                return _bootSoftwareVersionLabel;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, out object res))
                return (string)res;
            return _bootSoftwareVersionLabel;
        }
        internal set
        {
            _bootSoftwareVersionLabel = value;
            ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL, value);
        }
    }
    public BootSoftwareVersionModule(uint bootSoftwareVersionId, string bootSoftwareVersionLabel) : base(
        _moduleName,
        _moduleParameters)
    {
        _bootSoftwareVersionId = bootSoftwareVersionId;
        _bootSoftwareVersionLabel = bootSoftwareVersionLabel;
    }
    public BootSoftwareVersionModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
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