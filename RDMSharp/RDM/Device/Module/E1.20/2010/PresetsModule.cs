using RDMSharp.PayloadObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RDMSharp.RDM.Device.Module;

public class PresetsModule : AbstractModule
{
    private const string _moduleName = "Presets";
    private const string _moduleDisplayName = "Presets";
    private static readonly ERDM_Parameter[] _moduleParameters = new ERDM_Parameter[]
    {
        ERDM_Parameter.PRESET_PLAYBACK,
        ERDM_Parameter.CAPTURE_PRESET
    };
    public override string DisplayName => _moduleDisplayName;

    private bool initialized = false;
    private RDMPresetPlayback currentPlayBack;
    public RDMPresetPlayback? CurrentPlayback
    {
        get
        {
            if (ParentDevice is null)
                return currentPlayBack;
            if (ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PRESET_PLAYBACK, out object res))
                return (RDMPresetPlayback)res;
            return currentPlayBack;
        }
        set
        {
            if (value is null)
                return;

            // compare with the stored value of the device, the field is outdated after a SET by RDM
            if (initialized)
                if (Object.Equals(CurrentPlayback, value))
                    return;

            currentPlayBack = value;
            initialized = true;
            ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.PRESET_PLAYBACK, value);

            if (ParentRemoteDevice is not null)
                _ = SetCurrentPreset(value);
        }
    }

    public readonly IReadOnlyCollection<GeneratedPreset> _generatedPresets = null;
    public IReadOnlyCollection<IPreset> Presets
    {
        get
        {
            if (_generatedPresets is not null)
                return _generatedPresets;

            return Array.Empty<IPreset>();
        }
    }

    private readonly RDMPresetPlayback _initialValue = new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0);


    public PresetsModule(params GeneratedPreset[] presets) : base(
        _moduleName,
        _moduleParameters)
    {
        _generatedPresets = (presets ?? Array.Empty<GeneratedPreset>()).ToList().AsReadOnly();
    }
    public PresetsModule(AbstractRemoteRDMDevice remoteDevice) : base(
        remoteDevice,
        _moduleName,
        _moduleParameters)
    {
    }
    protected PresetsModule(string name, ERDM_Parameter[] supportedParameters, params GeneratedPreset[] presets) : base(
        name,
        supportedParameters)
    {
        _generatedPresets = (presets ?? Array.Empty<GeneratedPreset>()).ToList().AsReadOnly();
    }
    protected PresetsModule(AbstractRemoteRDMDevice remoteDevice, string name, params ERDM_Parameter[] supportedParameters) : base(
        remoteDevice,
        name,
        supportedParameters)
    {
    }

    protected override void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedPresets is not null && _generatedPresets.Count != 0)
        {
            var persDesc = new ConcurrentDictionary<object, object>();
            foreach (var gPreset in _generatedPresets)
                if (!persDesc.TryAdd(gPreset.ID, createPresetStatus(gPreset)))
                    throw new Exception($"{gPreset.ID} already used as {nameof(gPreset.ID)}");

            device.setParameterValue(ERDM_Parameter.PRESET_STATUS, persDesc);

            foreach (var gPreset in _generatedPresets)
                gPreset.PropertyChanged += GeneratedPreset_PropertyChanged;
        }
        ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.PRESET_PLAYBACK, _initialValue);
    }

    private static GetPresetStatus createPresetStatus(GeneratedPreset preset)
    {
        // E1.37-1 5.3: if a Preset is NOT_PROGRAMMED all timing information in the response shall be zero
        if (preset.Programmed == ERDM_PresetProgrammed.NOT_PROGRAMMED)
            return new GetPresetStatus(preset.ID, 0, 0, 0, preset.Programmed);

        return new GetPresetStatus(preset.ID, preset.UpFadeTime, preset.DownFadeTime, preset.WaitTime, preset.Programmed);
    }
    private void GeneratedPreset_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is GeneratedPreset preset)
            updatePresetStatus(preset);
    }
    /// <summary>
    /// Mirrors the current state of <paramref name="preset"/> into the PRESET_STATUS parameter of the generated device.
    /// </summary>
    private void updatePresetStatus(GeneratedPreset preset)
    {
        if (ParentGeneratedDevice is null)
            return;
        if (!ParentGeneratedDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.PRESET_STATUS, out object res))
            return;
        if (res is not ConcurrentDictionary<object, object> dict)
            return;

        dict[preset.ID] = createPresetStatus(preset);
        ParentGeneratedDevice.setParameterValue(ERDM_Parameter.PRESET_STATUS, dict, preset.ID);
    }

    protected GeneratedPreset GetGeneratedPreset(ushort sceneId)
    {
        return _generatedPresets?.FirstOrDefault(p => p.ID == sceneId);
    }

    protected static RDMMessage CreateResponse(RDMMessage request, ERDM_NackReason? nackReason = null)
    {
        var response = nackReason.HasValue ? new RDMMessage(nackReason.Value) : new RDMMessage();
        response.SourceUID = request.DestUID;
        response.DestUID = request.SourceUID;
        response.SubDevice = request.SubDevice;
        response.Command = request.Command | ERDM_Command.RESPONSE;
        response.Parameter = request.Parameter;
        return response;
    }

    /// <summary>
    /// Checks (and normalizes) the times of a Preset. The base implementation knows no limits and accepts everything.
    /// </summary>
    /// <param name="upFadeTime">Up Fade Time in seconds</param>
    /// <param name="downFadeTime">Down Fade Time in seconds, may be replaced by the Up Fade Time if split times are not supported</param>
    /// <param name="waitTime">Wait Time in seconds</param>
    /// <returns><c>false</c> if the times are out of range</returns>
    protected virtual bool ValidatePresetTimes(ref double upFadeTime, ref double downFadeTime, double waitTime)
    {
        return true;
    }

    /// <summary>
    /// SET PRESET_PLAYBACK: PDL has to be 3 and the mode has to be OFF, ALL or the number of an existing Scene.
    /// Returns <c>null</c> if the request is valid, so the generic handling stores the new value.
    /// </summary>
    private RDMMessage validatePresetPlayback(RDMMessage message)
    {
        if (message.PDL != RDMPresetPlayback.PDL || message.Value is not RDMPresetPlayback playback)
            return CreateResponse(message, ERDM_NackReason.FORMAT_ERROR);

        ushort mode = playback.Mode;
        if (mode == (ushort)ERDM_PresetPlayback.OFF || mode == (ushort)ERDM_PresetPlayback.ALL)
            return null;
        if (_generatedPresets is null || _generatedPresets.Count == 0)
            return null; // no Presets known, so there is nothing to compare with
        if (GetGeneratedPreset(mode) is null)
            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

        return null;
    }
    protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
    {
        if (parameter == ERDM_Parameter.PRESET_PLAYBACK)
            OnPropertyChanged(nameof(CurrentPlayback));
    }

    public async Task<bool> SetCurrentPreset(RDMPresetPlayback preset)
    {
        return await ParentRemoteDevice.SetParameter(ERDM_Parameter.PRESET_PLAYBACK, preset);
    }
    public async Task<bool> CapturePreset(RDMCapturePreset preset)
    {
        return await ParentRemoteDevice.SetParameter(ERDM_Parameter.CAPTURE_PRESET, preset);
    }

    public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
    {
        if (this.ParentGeneratedDevice is not null)
        {
            switch (parameter)
            {
                case ERDM_Parameter.CAPTURE_PRESET:
                    return true;
                case ERDM_Parameter.PRESET_PLAYBACK:
                    return command == ERDM_Command.SET_COMMAND;
            }
        }
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (this.ParentGeneratedDevice is not null)
        {
            if (message.Parameter == ERDM_Parameter.PRESET_PLAYBACK && message.Command == ERDM_Command.SET_COMMAND)
                return validatePresetPlayback(message);

            if (message.Parameter == ERDM_Parameter.CAPTURE_PRESET)
            {
                if (message.Command == ERDM_Command.GET_COMMAND)
                {
                    return new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        SubDevice = message.SubDevice,
                        Command = ERDM_Command.GET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                }
                else if (message.Command == ERDM_Command.SET_COMMAND)
                {
                    if ((message.PDL == RDMCapturePreset.PDL_small || message.PDL == RDMCapturePreset.PDL_big) && message.Value is RDMCapturePreset preset)
                    {
                        GeneratedPreset generatedPreset = null;
                        if (_generatedPresets is not null && _generatedPresets.Count != 0)
                        {
                            generatedPreset = GetGeneratedPreset(preset.SceneId);
                            if (generatedPreset is null)
                                return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);
                            if (generatedPreset.IsReadOnly)
                                return CreateResponse(message, ERDM_NackReason.WRITE_PROTECT);
                        }
                        double upFadeTime = preset.UpFadeTime;
                        double downFadeTime = preset.DownFadeTime;
                        if (message.PDL == RDMCapturePreset.PDL_big && !ValidatePresetTimes(ref upFadeTime, ref downFadeTime, preset.WaitTime))
                            return CreateResponse(message, ERDM_NackReason.DATA_OUT_OF_RANGE);

                        try
                        {
                            this.OnCapturePreset(preset);
                            if (generatedPreset is not null)
                            {
                                if (message.PDL == RDMCapturePreset.PDL_big)
                                {
                                    generatedPreset.UpFadeTime = upFadeTime;
                                    generatedPreset.DownFadeTime = downFadeTime;
                                    generatedPreset.WaitTime = preset.WaitTime;
                                }
                                generatedPreset.Programmed = ERDM_PresetProgrammed.PROGRAMMED;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger?.LogError(ex);
                            return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                SubDevice = message.SubDevice,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        return new RDMMessage()
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            SubDevice = message.SubDevice,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                    }
                    else
                        return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                        {
                            SourceUID = message.DestUID,
                            DestUID = message.SourceUID,
                            SubDevice = message.SubDevice,
                            Command = ERDM_Command.SET_COMMAND_RESPONSE,
                            Parameter = message.Parameter
                        };
                }
            }
        }
        return base.handleRequest(message);
    }

    protected virtual void OnCapturePreset(RDMCapturePreset preset)
    {
    }
}