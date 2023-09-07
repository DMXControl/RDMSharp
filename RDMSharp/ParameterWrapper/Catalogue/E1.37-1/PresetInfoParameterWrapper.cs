namespace RDMSharp.ParameterWrapper
{
    public sealed class PresetInfoParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMPresetInfo>
    {
        public PresetInfoParameterWrapper() : base(ERDM_Parameter.PRESET_INFO)
        {
        }
        public override string Name => "Preset Info";
        public override string Description => "This parameter is used to retrieve a variety of preset related information that describes the preset capabilities of the device.";

        protected override RDMPresetInfo getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetInfo.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMPresetInfo value)
        {
            return value.ToPayloadData();
        }
    }
}