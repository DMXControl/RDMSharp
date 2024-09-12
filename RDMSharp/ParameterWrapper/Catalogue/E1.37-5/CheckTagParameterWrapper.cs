namespace RDMSharp.ParameterWrapper
{
    public sealed class CheckTagParameterWrapper : AbstractRDMSetParameterWrapper<string,bool>
    {
        public CheckTagParameterWrapper() : base(ERDM_Parameter.CHECK_TAG)
        {
        }
        public override string Name => "Check Tag";
        public override string Description => "The CHECK_TAG parameter allows a Controller to test if a tag is present on a Responder.";

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData, 32);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            return Tools.ValueToData(value, 32);
        }

        protected override bool setResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
        }
    }
}