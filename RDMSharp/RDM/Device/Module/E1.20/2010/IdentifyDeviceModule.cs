using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class IdentifyDeviceModule : AbstractSimpleBooleanModule
{
    private const string _moduleName = "IdentifyDevice";
    private const string _moduleDisplayName = "Identify Device";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.IDENTIFY_DEVICE;

    public override string DisplayName => _moduleDisplayName;

    public bool? Identify
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

    public IdentifyDeviceModule() : base(
        false,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public IdentifyDeviceModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(Identify));
    }


    public async Task<bool> SetIdentify(bool identify)
    {
        return await base.SetValue(identify);
    }
}