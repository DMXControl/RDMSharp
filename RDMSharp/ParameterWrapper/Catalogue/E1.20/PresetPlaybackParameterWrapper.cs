namespace RDMSharp.ParameterWrapper
{
    public sealed class PresetPlaybackParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMPresetPlayback, RDMPresetPlayback>
    {
        public PresetPlaybackParameterWrapper() : base(ERDM_Parameter.PRESET_PLAYBACK)
        {
        }
        public override string Name => "Preset Playback";
        public override string Description => "This parameter is used to recall pre-recorded Presets.";

        protected override RDMPresetPlayback getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetPlayback.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMPresetPlayback value)
        {
            return Tools.ValueToData(value);
        }

        protected override RDMPresetPlayback setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetPlayback.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMPresetPlayback presetPlayback)
        {
            return presetPlayback.ToPayloadData();
        }
    }
}