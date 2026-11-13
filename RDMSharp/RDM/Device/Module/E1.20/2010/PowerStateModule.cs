using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class PowerStateModule : AbstractSimpleEnumModule<ERDM_PowerState>
{
    private const string _moduleName = "PowerState";
    private const string _moduleDisplayName = "Power State";
    private static readonly ERDM_Parameter _moduleParameter = ERDM_Parameter.POWER_STATE;

    public sealed override string DisplayName => _moduleDisplayName;

    public ERDM_PowerState? PowerState
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

    public PowerStateModule(ERDM_PowerState initialPowerState) : base(
        initialPowerState,
        _moduleName,
        _moduleParameter)
    {
    }
    public PowerStateModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (string.Equals(nameof(Value), propertyName))
            OnPropertyChanged(nameof(PowerState));
    }


    public async Task<bool> SetPowerState(ERDM_PowerState powerState)
    {
        return await base.SetValue(powerState);
    }
}