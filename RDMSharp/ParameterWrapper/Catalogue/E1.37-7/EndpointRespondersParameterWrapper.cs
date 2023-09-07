namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointRespondersParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<GetEndpointRespondersResponse>
    {
        public EndpointRespondersParameterWrapper() : base(ERDM_Parameter.ENDPOINT_RESPONDERS)
        {
        }
        public override string Name => "Endpoint Responders";
        public override string Description => "This parameter returns a packed list of responder UIDs associated with a given Endpoint ID number.";

        protected override GetEndpointRespondersResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetEndpointRespondersResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetEndpointRespondersResponse value)
        {
            return value.ToPayloadData();
        }
    }
}