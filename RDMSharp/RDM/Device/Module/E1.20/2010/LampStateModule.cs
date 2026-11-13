using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class LampStateModule : AbstractSimpleEnumModule<ERDM_LampState>
{
    private const string _moduleName = "LampState";
    private const string _moduleDisplayName = "Lamp State";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.LAMP_STATE;

    public sealed override string DisplayName => _moduleDisplayName;

    public ERDM_LampState? LampState
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

    public LampStateModule(ERDM_LampState initialLampState = ERDM_LampState.OFF) : base(
        initialLampState,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public LampStateModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(LampState));
    }


    public async Task<bool> SetLampState(ERDM_LampState lampState)
    {
        return await base.SetValue(lampState);
    }
}