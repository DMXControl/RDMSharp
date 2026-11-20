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
        if (this.ParentGeneratedDevice is not null)
        {
            switch (parameter)
            {
                case ERDM_Parameter.PRESET_STATUS:
                    return command == ERDM_Command.GET_COMMAND || command == ERDM_Command.SET_COMMAND;
                case ERDM_Parameter.PRESET_INFO:
                case ERDM_Parameter.PRESET_MERGEMODE:
                    return command == ERDM_Command.SET_COMMAND;
            }
        }
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (this.ParentGeneratedDevice is not null)
        {
            switch (message.Parameter)
            {
                case ERDM_Parameter.PRESET_STATUS when message.Command == ERDM_Command.SET_COMMAND:
                    return handleSetPresetStatus(message);

                case ERDM_Parameter.PRESET_STATUS when message.Command == ERDM_Command.GET_COMMAND:
                    // valid requests (null) are answered by the generic handling
                    return message.ParameterData is { Length: GET_PRESET_STATUS_PDL } ? null : CreateResponse(message, ERDM_NackReason.FORMAT_ERROR);

                case ERDM_Parameter.PRESET_INFO when message.Command == ERDM_Command.SET_COMMAND:
                    // PRESET_INFO is GET only (E1.37-1 5.2)
                    return CreateResponse(message, ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS);

                case ERDM_Parameter.PRESET_MERGEMODE when message.Command == ERDM_Command.SET_COMMAND:
                    return validateMergeMode(message);
            }
        }
        return base.handleRequest(message);
    }

    /// <summary>
    /// SET PRESET_MERGEMODE: PDL has to be 1 and the value has to be one of Table A-3 (0x00 - 0x03, 0xFF).
    /// Returns <c>null</c> if the request is valid, so the generic handling stores the new value.
    /// </summary>
    private static RDMMessage validateMergeMode(RDMMessage message)
    {
        byte[] data = message.ParameterData;
        if (data is null || data.Length != 1)
            return CreateResponse(message, ERDM_NackReason.FORMAT_ERROR);

        byte mode = data[0];
        if (mode > (byte)ERDM_MergeMode.DMX_ONLY && mode != (byte)ERDM_MergeMode.OTHER)
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

        return null;
    }

    //protected virtual void OnCapturePreset(RDMCapturePreset preset)
    //{
    //}

    private const int GET_PRESET_STATUS_PDL = 2;
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
            return CreateResponse(message, ERDM_NackReason.FORMAT_ERROR);

        ushort sceneId = status.SceneId;
        double upFade = status.UpFadeTime;
        double downFade = status.DownFadeTime;
        double waitTime = status.WaitTime;
        bool clearPreset = status.Clear;

        var presetInfo = PresetInfo ?? _initialPresetInfo;

        if (sceneId == 0 || sceneId == 0xFFFF)
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
        if (presetInfo is not null && sceneId > presetInfo.MaximumSceneNumber)
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

        var preset = GetGeneratedPreset(sceneId);
        if (preset is null)
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
        if (preset.IsReadOnly)
            return CreateResponse(message, ERDM_NackReason.WRITE_PROTECT);

        // Clearing resets all timing information to zero, the time fields of the request are ignored (E1.37-1 5.3)
        if (clearPreset)
        {
            preset.UpFadeTime = 0;
            preset.DownFadeTime = 0;
            preset.WaitTime = 0;
            preset.Programmed = ERDM_PresetProgrammed.NOT_PROGRAMMED;
            return CreateResponse(message);
        }

        if (!ValidatePresetTimes(ref upFade, ref downFade, waitTime))
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

        preset.UpFadeTime = upFade;
        preset.DownFadeTime = downFade;
        preset.WaitTime = waitTime;

        return CreateResponse(message);
    }

    protected override bool ValidatePresetTimes(ref double upFadeTime, ref double downFadeTime, double waitTime)
    {
        var presetInfo = PresetInfo ?? _initialPresetInfo;
        if (presetInfo is null)
            return true;

        // Without split times the device only knows one fade time, the Up Fade Time is used for both (E1.37-1 5.2)
        if (!presetInfo.SplitTimesSupported)
            downFadeTime = upFadeTime;

        return isInRange(upFadeTime, presetInfo.MinimumPresetFadeTimeSupported, presetInfo.MaximumPresetFadeTimeSupported)
            && isInRange(downFadeTime, presetInfo.MinimumPresetFadeTimeSupported, presetInfo.MaximumPresetFadeTimeSupported)
            && isInRange(waitTime, presetInfo.MinimumPresetWaitTimeSupported, presetInfo.MaximumPresetWaitTimeSupported);
    }

    /// <summary>
    /// Compares in tenths of a second, the resolution used on the wire, to avoid floating point issues.
    /// </summary>
    private static bool isInRange(double valueSeconds, double minimumSeconds, double maximumSeconds)
    {
        long value = (long)Math.Round(valueSeconds * 10);
        long min = (long)Math.Round(minimumSeconds * 10);
        long max = (long)Math.Round(maximumSeconds * 10);
        return value >= min && value <= max;
    }
}