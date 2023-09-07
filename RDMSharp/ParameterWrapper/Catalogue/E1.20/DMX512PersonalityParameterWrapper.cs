namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512PersonalityParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMDMXPersonality, byte>
    {
        public DMX512PersonalityParameterWrapper() : base(ERDM_Parameter.DMX_PERSONALITY)
        {
        }
        public override string Name => "DMX512 Personality";
        public override string Description => 
            "This parameter is used to set the responder’s DMX512 Personality. Many devices such as " +
            "moving lights have different DMX512 “Personalities”. Many RDM parameters may be affected by" +
            "changing personality.";

        protected override RDMDMXPersonality getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDMXPersonality.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMDMXPersonality value)
        {
            return value.ToPayloadData();
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte value)
        {
            return Tools.ValueToData(value);
        }
    }
}