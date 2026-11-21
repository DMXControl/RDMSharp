using RDMSharp.PayloadObject;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

/// <summary>
/// Common handling of DMX_FAIL_MODE (E1.37-1 3.4) and DMX_STARTUP_MODE (E1.37-1 3.5).
/// <list type="bullet">
/// <item>Scene # 0x0000 (PRESET_PLAYBACK_OFF) is always supported, other scenes need presets (<see cref="PresetsModule"/>) of the device,
/// a scene the device does not support is NACKed with DATA_OUT_OF_RANGE.</item>
/// <item>Delay and Hold Time outside of the limits (PRESET_INFO) are set to the minimum or maximum,
/// 0xFFFF (infinite) is set to the maximum, if infinite is not supported.</item>
/// <item>Without PRESET_INFO the device has no known limits and accepts every time.</item>
/// </list>
/// </summary>
public abstract class AbstractDMX_xxxx_ModeModule : AbstractModule
{
    private const ushort INFINITE = ushort.MaxValue;
    private const ushort SCENE_PRESET_PLAYBACK_OFF = 0x0000;
    private const ushort SCENE_PRESET_PLAYBACK_ALL = 0xFFFF;

    private readonly ERDM_Parameter _moduleParameter;
    private readonly RDMDMX_xxxx_Mode _initialMode;

    private bool initialized = false;
    private RDMDMX_xxxx_Mode _mode;
    public RDMDMX_xxxx_Mode Mode
    {
        get
        {
            if (ParentDevice is null)
                return _mode;
            if (ParentDevice.GetAllParameterValues().TryGetValue(_moduleParameter, out object res) && res is RDMDMX_xxxx_Mode mode)
                return mode;
            return _mode;
        }
        set
        {
            if (value is null)
                return;

            // compare with the stored value of the device, the field is outdated after a SET by RDM
            if (initialized)
                if (Object.Equals(Mode, value))
                    return;

            _mode = value;
            initialized = true;
            ParentGeneratedDevice?.setParameterValue(_moduleParameter, value);

            if (ParentRemoteDevice is not null)
                _ = SetMode(value);
        }
    }

    protected AbstractDMX_xxxx_ModeModule(
        RDMDMX_xxxx_Mode initialMode,
        string name,
        ERDM_Parameter moduleParameter) : base(
            name,
            moduleParameter)
    {
        _moduleParameter = moduleParameter;
        _initialMode = initialMode ?? new RDMDMX_xxxx_Mode();
    }
    protected AbstractDMX_xxxx_ModeModule(
        AbstractRemoteRDMDevice remoteDevice,
        string name,
        ERDM_Parameter moduleParameter) : base(
            remoteDevice,
            name,
            moduleParameter)
    {
        _moduleParameter = moduleParameter;
    }

    protected abstract bool IsInfiniteDelayTimeSupported(RDMPresetInfo presetInfo);
    protected abstract bool IsInfiniteHoldTimeSupported(RDMPresetInfo presetInfo);
    protected abstract (double? Minimum, double? Maximum) GetDelayTimeLimits(RDMPresetInfo presetInfo);
    protected abstract (double? Minimum, double? Maximum) GetHoldTimeLimits(RDMPresetInfo presetInfo);

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        this.Mode = _initialMode;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (parameter == _moduleParameter)
            OnPropertyChanged(nameof(Mode));
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (ParentGeneratedDevice is not null && parameter == _moduleParameter)
            return command == ERDM_Command.SET_COMMAND;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (message.Parameter == _moduleParameter && message.Command == ERDM_Command.SET_COMMAND)
        {
            if (message.PDL != RDMDMX_xxxx_Mode.PDL || message.Value is not RDMDMX_xxxx_Mode requested)
                return createResponse(message, ERDM_NackReason.FORMAT_ERROR);

            RDMPresetInfo presetInfo = null;
            if (ParentGeneratedDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PRESET_INFO, out object pi))
                presetInfo = pi as RDMPresetInfo;

            if (!isSceneSupported(requested.Scene, presetInfo))
                return createResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

            double delayTime = limit(requested.Delay, GetDelayTimeLimits(presetInfo), presetInfo is null || IsInfiniteDelayTimeSupported(presetInfo));
            double holdTime = limit(requested.HoldTime, GetHoldTimeLimits(presetInfo), presetInfo is null || IsInfiniteHoldTimeSupported(presetInfo));

            Mode = new RDMDMX_xxxx_Mode(requested.Scene, delayTime, holdTime, requested.Level);
            return createResponse(message);
        }
        return base.handleRequest(message);
    }

    private bool isSceneSupported(ushort scene, RDMPresetInfo presetInfo)
    {
        if (scene == SCENE_PRESET_PLAYBACK_OFF)
            return true;

        var presetsModule = ParentGeneratedDevice.Modules.OfType<PresetsModule>().FirstOrDefault();
        if (presetsModule is null)
            return false; // without presets there is no scene to play

        if (scene == SCENE_PRESET_PLAYBACK_ALL)
            return true;

        if (presetsModule._generatedPresets is not null && presetsModule._generatedPresets.Count != 0)
            return presetsModule._generatedPresets.Any(p => p.ID == scene);

        return presetInfo is null || scene <= presetInfo.MaximumSceneNumber;
    }

    /// <summary>
    /// Sets the time (seconds) to the minimum or maximum, if it is outside of the limits.
    /// </summary>
    private static double limit(double time, (double? Minimum, double? Maximum) limits, bool infiniteSupported)
    {
        double? minimum = knownLimit(limits.Minimum);
        double? maximum = knownLimit(limits.Maximum);

        if (isInfinite(time))
        {
            if (infiniteSupported)
                return time;
            return maximum ?? time;
        }
        if (minimum.HasValue && time < minimum.Value)
            return minimum.Value;
        if (maximum.HasValue && time > maximum.Value)
            return maximum.Value;
        return time;

        // 0xFFFF in PRESET_INFO means the limit is not reported
        static double? knownLimit(double? seconds)
        {
            if (!seconds.HasValue || seconds.Value < 0 || isInfinite(seconds.Value))
                return null;
            return seconds.Value;
        }
    }
    /// <summary>
    /// 0xFFFF (6553.5 seconds) is infinite, the times are compared in tenths of a second, the resolution of the payload.
    /// </summary>
    private static bool isInfinite(double seconds)
    {
        return Math.Round(seconds * 10) >= INFINITE;
    }

    private static RDMMessage createResponse(RDMMessage request, ERDM_NackReason? nackReason = null)
    {
        var response = nackReason.HasValue ? new RDMMessage(nackReason.Value) : new RDMMessage();
        response.SourceUID = request.DestUID;
        response.DestUID = request.SourceUID;
        response.SubDevice = request.SubDevice;
        response.Command = request.Command | ERDM_Command.RESPONSE;
        response.Parameter = request.Parameter;
        return response;
    }

    public async Task<bool> SetMode(RDMDMX_xxxx_Mode mode)
    {
        if (mode is null)
            throw new ArgumentNullException(nameof(mode));

        if (ParentGeneratedDevice is not null)
        {
            Mode = mode;
            return true;
        }
        if (ParentRemoteDevice is not null)
            return await ParentRemoteDevice.SetParameter(_moduleParameter, mode);
        return false;
    }
}
