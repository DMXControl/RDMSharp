namespace RDMSharp.RDM.Device.Module;

public sealed class DeviceModelDescriptionModule : AbstractModule
{
    private const string _moduleName = "DeviceModelDescription";
    private const string _moduleDisplayName = "Device Model Description";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_MODEL_DESCRIPTION;

    public override string DisplayName => _moduleDisplayName;

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
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION, value);
        }
    }
    public DeviceModelDescriptionModule(string manufacturerLabel) : base(
        _moduleName,
        _moduleParameter)
    {
        _deviceModelDescriptionLabel = manufacturerLabel;
    }
    public DeviceModelDescriptionModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
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