namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// POWER_OFF_READY (E1.37-5 4.6): for devices that need a shutdown period before power is removed.
/// GET only, <c>true</c> if power can be removed from the device without causing an issue.
/// A change is reported via the queued messages, as the standard asks for after the shutdown.
/// </summary>
public sealed class PowerOffReadyModule : AbstractSimpleBooleanModule
{
    private const string _moduleName = "PowerOffReady";
    private const string _moduleDisplayName = "Power Off Ready";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.POWER_OFF_READY;

    public sealed override string DisplayName => _moduleDisplayName;

    public bool? PowerOffReady
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

    public PowerOffReadyModule(bool initialPowerOffReady = false) : base(
        initialPowerOffReady,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET)
    {
    }
    public PowerOffReadyModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(PowerOffReady));
    }
}
