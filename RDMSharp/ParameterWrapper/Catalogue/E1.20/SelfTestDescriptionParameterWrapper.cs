namespace RDMSharp.ParameterWrapper
{
    public sealed class SelfTestDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMSelfTestDescription>, IRDMBlueprintParameterWrapper
    {
        public SelfTestDescriptionParameterWrapper() : base(ERDM_Parameter.SELF_TEST_DESCRIPTION)
        {
        }
        public override string Name => "Self Test Description";
        public override string Description => "This parameter is used to get a descriptive ASCII text label for a given Self Test Operation. The label may be up to 32 characters.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[0];
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte selftestID)
        {
            return Tools.ValueToData(selftestID);
        }

        protected override RDMSelfTestDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSelfTestDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMSelfTestDescription value)
        {
            return value.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            return new RequestRange<byte>(0, byte.MaxValue);
        }
    }
}