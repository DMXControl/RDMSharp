using RDMSharp.PayloadObject;
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
        //if (this.ParentGeneratedDevice is not null)
        //{
        //    switch (parameter)
        //    {
        //        case ERDM_Parameter.CAPTURE_PRESET:
        //            return true;
        //    }
        //}
        return base.IsHandlingParameter(parameter, command);
    }
    protected override RDMMessage handleRequest(RDMMessage message)
    {
        //if (this.ParentGeneratedDevice is not null)
        //{
        //    if (message.Parameter == ERDM_Parameter.CAPTURE_PRESET)
        //    {
        //        if (message.Command == ERDM_Command.GET_COMMAND)
        //        {
        //            return new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS)
        //            {
        //                SourceUID = message.DestUID,
        //                DestUID = message.SourceUID,
        //                SubDevice = message.SubDevice,
        //                Command = ERDM_Command.GET_COMMAND_RESPONSE,
        //                Parameter = message.Parameter
        //            };
        //        }
        //        else if (message.Command == ERDM_Command.SET_COMMAND)
        //        {
        //            if ((message.PDL >= 2 || message.PDL <= 8) && message.Value is RDMCapturePreset preset)
        //            {
        //                try
        //                {
        //                    this.OnCapturePreset(preset);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Logger?.LogError(ex);
        //                    return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
        //                    {
        //                        SourceUID = message.DestUID,
        //                        DestUID = message.SourceUID,
        //                        SubDevice = message.SubDevice,
        //                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
        //                        Parameter = message.Parameter
        //                    };
        //                }
        //                return new RDMMessage()
        //                {
        //                    SourceUID = message.DestUID,
        //                    DestUID = message.SourceUID,
        //                    SubDevice = message.SubDevice,
        //                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
        //                    Parameter = message.Parameter
        //                };
        //            }
        //            else
        //                return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
        //                {
        //                    SourceUID = message.DestUID,
        //                    DestUID = message.SourceUID,
        //                    SubDevice = message.SubDevice,
        //                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
        //                    Parameter = message.Parameter
        //                };
        //        }
        //    }
        //}
        return base.handleRequest(message);
    }

    //protected virtual void OnCapturePreset(RDMCapturePreset preset)
    //{
    //}
}