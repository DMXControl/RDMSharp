namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointListChangeParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<uint>
    {
        public EndpointListChangeParameterWrapper() : base(ERDM_Parameter.ENDPOINT_LIST_CHANGE)
        {
        }
        public override string Name => "Endpoint Responder List Change";
        public override string Description =>
            "This parameter returns a unique List Change Number as a means for Controllers to identify if the " +
            "Endpoint Responder List has changed.";

        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }
    }
}