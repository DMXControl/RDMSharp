using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// BURN_IN (E1.37-1 3.10): burn-in for e.g. fluorescent lamps after a lamp change.
/// GET returns the remaining burn-in time in hours (0-255), SET sets the burn-in time and starts the burn-in,
/// SET 0 aborts a running burn-in.
/// The device itself runs the burn-in and counts down <see cref="HoursRemaining"/>.
/// </summary>
public sealed class BurnInModule : AbstractSimpleNumericParameterIDModule<byte>
{
    private const string _moduleName = "BurnIn";
    private const string _moduleDisplayName = "Burn-In";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.BURN_IN;
    public const byte Abort = 0;

    public sealed override string DisplayName => _moduleDisplayName;

    /// <summary>
    /// Remaining burn-in time in hours, 0 if no burn-in is running.
    /// </summary>
    public byte? HoursRemaining
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

    public BurnInModule(byte initialHoursRemaining = Abort) : base(
        initialHoursRemaining,
        _moduleName,
        _moduleParameter,
        ERDM_CommandClass.GET | ERDM_CommandClass.SET)
    {
    }
    public BurnInModule(AbstractRemoteRDMDevice remoteDevice) : base(
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
            OnPropertyChanged(nameof(HoursRemaining));
    }

    /// <summary>
    /// Sets the burn-in time and starts the burn-in on the remote device.
    /// </summary>
    public async Task<bool> StartBurnIn(byte hours)
    {
        return await base.SetValue(hours);
    }
    /// <summary>
    /// Aborts a running burn-in on the remote device.
    /// </summary>
    public async Task<bool> AbortBurnIn()
    {
        return await base.SetValue(Abort);
    }
}
