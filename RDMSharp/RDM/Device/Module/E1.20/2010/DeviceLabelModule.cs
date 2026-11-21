using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class DeviceLabelModule : AbstractSimpleStringModule
{
    private const string _moduleName = "DeviceLabel";
    private const string _moduleDisplayName = "Device Label";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DEVICE_LABEL;

    public override string DisplayName => _moduleDisplayName;

    public string DeviceLabel
    {
        get
        {
            return base.Value;
        }
        set
        {
            base.Value = value;
        }
    }
    public DeviceLabelModule(string deviceLabel) : base(
        deviceLabel,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public DeviceLabelModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(DeviceLabel));
    }

    public async Task<bool> SetDeviceLabel(string deviceLabel)
    {
        return await base.SetValue(deviceLabel);
    }
}