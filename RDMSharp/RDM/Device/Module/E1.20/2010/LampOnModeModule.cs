using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class LampOnModeModule : AbstractSimpleEnumModule<ERDM_LampMode>
{
    private const string _moduleName = "LampOnMode";
    private const string _moduleDisplayName = "Lamp On Mode";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.LAMP_ON_MODE;

    public sealed override string DisplayName => _moduleDisplayName;

    public ERDM_LampMode? LampOnMode
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

    public LampOnModeModule(ERDM_LampMode initialLampOnMode = ERDM_LampMode.ON_MODE_OFF) : base(
        initialLampOnMode,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public LampOnModeModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(LampOnMode));
    }


    public async Task<bool> SetLampOnMode(ERDM_LampMode lampOnMode)
    {
        return await base.SetValue(lampOnMode);
    }
}