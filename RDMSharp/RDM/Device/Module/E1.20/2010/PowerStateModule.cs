using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public sealed class PowerStateModule : AbstractModule
{
    private const string _moduleName = "PowerState";
    private const string _moduleDisplayName = "Power State";
    private static readonly ERDM_Parameter _moduleParameter = ERDM_Parameter.POWER_STATE;

    public override string DisplayName => _moduleDisplayName;

    private ERDM_PowerState? powerState;
    public ERDM_PowerState? PowerState
    {
        get
        {
            return powerState;
        }
        set
        {
            if (value is null)
                return;

            if (powerState == value)
                return;

            bool initial = powerState is null;
            var backup = powerState;
            powerState = value;

            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.POWER_STATE, powerState.Value);

            if (initial)
                return;

            if (ParentRemoteDevice is not null)
            {
                Task.Run(async () =>
                {
                    if (await SetPowerState(value.Value))
                        OnPropertyChanged(nameof(PowerState));
                    else
                    {
                        powerState = backup;
                        OnPropertyChanged(nameof(PowerState));
                    }
                });
            }
        }
    }

    private ERDM_PowerState _initialPowerState;

    public PowerStateModule(ERDM_PowerState initialPowerState) : base(
        _moduleName,
        _moduleParameter)
    {
        _initialPowerState = initialPowerState;
    }
    public PowerStateModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
        if (ParentRemoteDevice.ParameterValues.TryGetValue(ERDM_Parameter.POWER_STATE, out object val) && val is ERDM_PowerState powerState)
            PowerState = powerState;
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        await SetPowerState(_initialPowerState);
    }

    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (ParentRemoteDevice is null)
            return;

        if (_moduleParameter != parameter)
            return;

        switch (parameter)
        {
            case ERDM_Parameter.POWER_STATE when newValue is ERDM_PowerState powerState:
                PowerState = powerState;
                break;
        }
    }


    public async Task<bool> SetPowerState(ERDM_PowerState powerState)
    {
        if (ParentGeneratedDevice is not null)
            PowerState = powerState;
        if (ParentRemoteDevice is not null)
            return await ParentRemoteDevice.SetParameter(ERDM_Parameter.POWER_STATE, powerState);

        return true;
    }
}