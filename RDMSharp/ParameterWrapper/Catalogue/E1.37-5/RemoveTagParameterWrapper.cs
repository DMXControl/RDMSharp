namespace RDMSharp.ParameterWrapper
{
    public sealed class RemoveTagParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<string>
    {
        public RemoveTagParameterWrapper() : base(ERDM_Parameter.REMOVE_TAG)
        {
        }
        public override string Name => "Remove Tag";
        public override string Description => "This parameter removes a tag from a Responder.";

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