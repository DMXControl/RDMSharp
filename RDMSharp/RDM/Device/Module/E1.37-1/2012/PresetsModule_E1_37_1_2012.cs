using RDMSharp.PayloadObject;
using System;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public class PresetsModule_E1_37_1_2012 : PresetsModule
{
    private const string _moduleName = "Presets_E1.37-1 2012";
    private const string _moduleDisplayName = "Presets";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.PRESET_PLAYBACK,
        ERDM_Parameter.CAPTURE_PRESET,
        ERDM_Parameter.PRESET_INFO,
        ERDM_Parameter.PRESET_STATUS,
        ERDM_Parameter.PRESET_MERGEMODE
    };
    public override string DisplayName => _moduleDisplayName;

    private bool initialized = false;
    public RDMPresetInfo PresetInfo
    {
        get
        {
            if (ParentDevice is null)
                return null;
            object res;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PRESET_INFO, out res))
                return (RDMPresetInfo)res;
            return null;
        }
        internal set
        {
            if (ParentGeneratedDevice is not null)
                ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PRESET_INFO, value);
        }
    }

    private ERDM_MergeMode currentMergeMode;
    public ERDM_MergeMode? CurrentMergeMode
    {
        get
        {
            if (ParentDevice is null)
                return currentMergeMode;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PRESET_MERGEMODE, out object res))
                return (ERDM_MergeMode)res;
            return currentMergeMode;
        }
        set
        {
            if (value is null)
                return;

            if (initialized)
                if (bool.Equals(currentMergeMode, value))
                    return;

            currentMergeMode = value.Value;
            initialized = true;
            ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.PRESET_MERGEMODE, value);

            if (ParentRemoteDevice is not null)
                _ = SetCurrentMergeMode(value.Value);
        }
    }

    private readonly RDMPresetInfo _initialPresetInfo;
    private readonly ERDM_MergeMode _initialMergeMode = ERDM_MergeMode.DEFAULT;


    public PresetsModule_E1_37_1_2012(RDMPresetInfo presetInfo, ERDM_MergeMode initialMergeMode = ERDM_MergeMode.DEFAULT, params GeneratedPreset[] presets) : base(
        _moduleName,
        _moduleParameters,
        presets)
    {
        _initialPresetInfo = presetInfo;
        _initialMergeMode = initialMergeMode;
    }
    public PresetsModule_E1_37_1_2012(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        PresetInfo = _initialPresetInfo;
        base.OnParentGeneratedDeviceChanged(device);
        ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.PRESET_MERGEMODE, _initialMergeMode);
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        base.ParameterChanged(parameter, newValue, index);
        if (parameter == ERDM_Parameter.PRESET_MERGEMODE)
            OnPropertyChanged(nameof(CurrentMergeMode));
    }

    public async Task<bool> SetCurrentMergeMode(ERDM_MergeMode mergeMode)
    {
        return await ParentRemoteDevice.SetParameter(ERDM_Parameter.PRESET_MERGEMODE, mergeMode);
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (this.ParentGeneratedDevice is not null
            && parameter == ERDM_Parameter.PRESET_STATUS
            && command == ERDM_Command.SET_COMMAND)
            return true;
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (this.ParentGeneratedDevice is not null
            && message.Parameter == ERDM_Parameter.PRESET_STATUS
            && message.Command == ERDM_Command.SET_COMMAND)
            return handleSetPresetStatus(message);
        return base.handleRequest(message);
    }

    //protected virtual void OnCapturePreset(RDMCapturePreset preset)
    //{
    //}

    private const int SET_PRESET_STATUS_PDL = 9;

    /// <summary>
    /// SET PRESET_STATUS (E1.37-1):
    /// scene_num (uint16), up_fade_time, down_fade_time, wait_time (uint16, tenths of a second), clear_preset (uint8).
    /// The payload is parsed here directly, so an invalid scene number is answered with DATA_OUT_OF_RANGE
    /// instead of failing in the generic metadata parser.
    /// </summary>
    private RDMMessage handleSetPresetStatus(RDMMessage message)
    {
        byte[] data = message.ParameterData;
        if (data is null || data.Length != SET_PRESET_STATUS_PDL || message.Value is not SetPresetStatus status)
            return CreateSetResponse(message, ERDM_NackReason.FORMAT_ERROR);

        ushort sceneId = status.SceneId;
        double upFade = status.UpFadeTime;
        double downFade = status.DownFadeTime;
        double waitTime = status.WaitTime;
        bool clearPreset = status.Clear;

        var presetInfo = PresetInfo ?? _initialPresetInfo;

        if (sceneId == 0 || sceneId == 0xFFFF)
            return CreateSetResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
        if (presetInfo is not null && sceneId > presetInfo.MaximumSceneNumber)
            return CreateSetResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

        var preset = GetGeneratedPreset(sceneId);
        if (preset is null)
            return CreateSetResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
        if (preset.IsReadOnly)
            return CreateSetResponse(message, ERDM_NackReason.WRITE_PROTECT);

        // Clearing ignores the time fields
        if (clearPreset)
        {
            preset.Programmed = ERDM_PresetProgrammed.NOT_PROGRAMMED;
            return CreateSetResponse(message);
        }

        // Without split times the device only knows one fade time, the down fade time is ignored
        if (presetInfo is not null && !presetInfo.SplitTimesSupported)
            downFade = upFade;

        if (presetInfo is not null)
        {
            if (!isInRange(upFade, presetInfo.MinimumPresetFadeTimeSupported, presetInfo.MaximumPresetFadeTimeSupported)
                || !isInRange(downFade, presetInfo.MinimumPresetFadeTimeSupported, presetInfo.MaximumPresetFadeTimeSupported)
                || !isInRange(waitTime, presetInfo.MinimumPresetWaitTimeSupported, presetInfo.MaximumPresetWaitTimeSupported))
                return CreateSetResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
        }

        preset.UpFadeTime = upFade;
        preset.DownFadeTime = downFade;
        preset.WaitTime = waitTime;

        return CreateSetResponse(message);
    }

    /// <summary>
    /// Compares in tenths of a second, the resolution used on the wire, to avoid floating point issues.
    /// </summary>
    private static bool isInRange(double value, double minimumSeconds, double maximumSeconds)
    {
        int min = (int)Math.Round(minimumSeconds);
        int max = (int)Math.Round(maximumSeconds);
        return value >= min && value <= max;
    }
}