using RDMSharp.PayloadObject;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// DMX_STARTUP_MODE (E1.37-1 3.5): behavior of the device when it starts up and the DMX512 control signal is absent.
/// There is no infinite Startup Delay, an infinite Hold Time is allowed, if PRESET_INFO reports it as supported.
/// </summary>
public sealed class DMXStartupModeModule : AbstractDMX_xxxx_ModeModule
{
    private const string _moduleName = "DMXStartupMode";
    private const string _moduleDisplayName = "DMX512 Startup Mode";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DMX_STARTUP_MODE;

    public override string DisplayName => _moduleDisplayName;

    public DMXStartupModeModule(RDMDMX_xxxx_Mode initialMode = null) : base(
        initialMode,
        _moduleName,
        _moduleParameter)
    {
    }
    public DMXStartupModeModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override bool IsInfiniteDelayTimeSupported(RDMPresetInfo presetInfo) => false;
    protected override bool IsInfiniteHoldTimeSupported(RDMPresetInfo presetInfo) => presetInfo.StartupInfiniteHoldTimeSupported;
    protected override (double? Minimum, double? Maximum) GetDelayTimeLimits(RDMPresetInfo presetInfo)
        => (presetInfo?.MinimumStartupDelayTimeSupported, presetInfo?.MaximumStartupDelayTimeSupported);
    protected override (double? Minimum, double? Maximum) GetHoldTimeLimits(RDMPresetInfo presetInfo)
        => (presetInfo?.MinimumStartupDelayHoldSupported, presetInfo?.MaximumStartupDelayHoldSupported);
}
