namespace RDMSharp.RDM.Device.Module;

public sealed class DeviceModelDescriptionModule : AbstractSimpleStringModule
{
    private const string _moduleName = "DeviceModelDescription";
    private const string _moduleDisplayName = "Device Model Description";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_MODEL_DESCRIPTION;

    public override string DisplayName => _moduleDisplayName;

    public string DeviceModelDescription
    {
        get
        {
            return base.Value;
        }
    }
    public DeviceModelDescriptionModule(string deviceModelDescription) : base(
        deviceModelDescription,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public DeviceModelDescriptionModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(DeviceModelDescription));
    }
}