namespace RDMSharp.ParameterWrapper
{
    public sealed class AddTagParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<string>
    {
        public AddTagParameterWrapper() : base(ERDM_Parameter.ADD_TAG)
        {
        }
        public override string Name => "Add Tag";
        public override string Description => "This parameter associates a tag with a Responder. If the tag already exists on the Responder, the Responder shall just return an ACK without modifying the tag in any way.";

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData, 32);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            return Tools.ValueToData(value, 32);
        }
    }
}