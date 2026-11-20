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

            if (initialized)
                if (bool.Equals(currentPlayBack, value))
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

            return _generatedPresets.ToList<IPreset>().AsReadOnly();
        }
    }
    public readonly byte PersonalitiesCount;

    private readonly RDMPresetPlayback _initialValue = new RDMPresetPlayback(ERDM_PresetPlayback.OFF, 0);


    public PresetsModule(params GeneratedPreset[] presets) : base(
        _moduleName,
        _moduleParameters)
    {
        _generatedPresets = presets.ToList().AsReadOnly();
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
    }
    protected PresetsModule(AbstractRemoteRDMDevice remoteDevice, string name, params ERDM_Parameter[] supportedParameters) : base(
        remoteDevice,
        name,
        supportedParameters)
    {
    }

    protected override async void OnParentGeneratedDeviceChanged(AbstractGeneratedRDMDevice device)
    {
        if (_generatedPresets.Count != 0)
        {
            var persDesc = new ConcurrentDictionary<object, object>();
            foreach (var gPreset in _generatedPresets)
                if (!persDesc.TryAdd(gPreset.ID, new RDMPresetStatus(gPreset.ID, gPreset.UpFadeTime, gPreset.DownFadeTime, gPreset.WaitTime, ERDM_PresetProgrammed.NOT_PROGRAMMED)))
                    throw new Exception($"{gPreset.ID} already used as {nameof(gPreset.ID)}");

            device.setParameterValue(ERDM_Parameter.PRESET_STATUS, persDesc);
        }
        ParentGeneratedDevice?.setParameterValue(ERDM_Parameter.PRESET_PLAYBACK, _initialValue);
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
            }
        }
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        if (this.ParentGeneratedDevice is not null)
        {
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
                    if ((message.PDL >= 2 || message.PDL <= 8) && message.Value is RDMCapturePreset preset)
                    {
                        try
                        {
                            this.OnCapturePreset(preset);
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