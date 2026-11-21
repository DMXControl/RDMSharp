using RDMSharp.PayloadObject;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// DMX_FAIL_MODE (E1.37-1 3.4): behavior of the device when the DMX512 control signal is lost.
/// Infinite Loss of Signal Delay and infinite Hold Time are allowed, if PRESET_INFO reports them as supported.
/// </summary>
public sealed class DMXFailModeModule : AbstractDMX_xxxx_ModeModule
{
    private const string _moduleName = "DMXFailMode";
    private const string _moduleDisplayName = "DMX512 Fail Mode";
    private const ERDM_Parameter _moduleParameter = ERDM_Parameter.DMX_FAIL_MODE;

    public override string DisplayName => _moduleDisplayName;

    public DMXFailModeModule(RDMDMX_xxxx_Mode initialMode = null) : base(
        initialMode,
        _moduleName,
        _moduleParameter)
    {
    }
    public DMXFailModeModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameter)
    {
    }

    protected override bool IsInfiniteDelayTimeSupported(RDMPresetInfo presetInfo) => presetInfo.DMX512FailInfiniteDelayTimeSupported;
    protected override bool IsInfiniteHoldTimeSupported(RDMPresetInfo presetInfo) => presetInfo.DMX512FailInfiniteHoldTimeSupported;
    protected override (double? Minimum, double? Maximum) GetDelayTimeLimits(RDMPresetInfo presetInfo)
        => (presetInfo?.MinimumDMX512FailDelayTimeSupported, presetInfo?.MaximumDMX512FailDelayTimeSupported);
    protected override (double? Minimum, double? Maximum) GetHoldTimeLimits(RDMPresetInfo presetInfo)
        => (presetInfo?.MinimumDMX512FailDelayHoldSupported, presetInfo?.MaximumDMX512FailDelayHoldSupported);
}
