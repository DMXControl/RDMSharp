namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointResponderListChangeParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<GetEndpointResponderListChangeResponse>
    {
        public EndpointResponderListChangeParameterWrapper() : base(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE)
        {
        }
        public override string Name => "Endpoint Responder List Change";
        public override string Description =>
            "This parameter returns a unique List Change Number as a means for Controllers to identify if the " +
            "Endpoint Responder List has changed.";

        protected override GetEndpointResponderListChangeResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetEndpointResponderListChangeResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetEndpointResponderListChangeResponse value)
        {
            return value.ToPayloadData();
        }
    }
}