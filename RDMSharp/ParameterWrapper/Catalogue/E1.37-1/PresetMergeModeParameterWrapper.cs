namespace RDMSharp.ParameterWrapper
{
    public sealed class PresetMergeModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_MergeMode, ERDM_MergeMode>
    {
        public PresetMergeModeParameterWrapper() : base(ERDM_Parameter.PRESET_MERGEMODE)
        {
        }
        public override string Name => "Preset Merge Mode";
        public override string Description =>
            "Normally a preset started with the PRESET_PLAYBACK message has priority over a DMX512 input signal. " +
            "On some devices this may not be the desired effect, and other merge modes may be offered.\r\n\r\n" +
            "This parameter is used to retrieve or change the preset merge mode.";

        protected override ERDM_MergeMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_MergeMode>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_MergeMode presetMergeMode)
        {
            return Tools.ValueToData(presetMergeMode);
        }

        protected override ERDM_MergeMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_MergeMode>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_MergeMode presetMergeMode)
        {
            return Tools.ValueToData(presetMergeMode);
        }
    }
}