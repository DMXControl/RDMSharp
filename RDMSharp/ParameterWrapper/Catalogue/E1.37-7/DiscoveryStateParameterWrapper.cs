namespace RDMSharp.ParameterWrapper
{
    public sealed class DiscoveryStateParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetDiscoveryStateResponse, SetDiscoveryStateRequest, ushort>
    {
        public DiscoveryStateParameterWrapper() : base(ERDM_Parameter.DISCOVERY_STATE)
        {
        }
        public override string Name => "Discovery State";
        public override string Description => 
            "This parameter is used to initiate E1.20 RDM Discovery ([RDM] Section 7) of RDM Responders " +
            "connected to the specified Endpoint or to get the status of the discovery process.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_LIST };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }

        protected override GetDiscoveryStateResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetDiscoveryStateResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetDiscoveryStateResponse discoveryState)
        {
            return discoveryState.ToPayloadData();
        }

        protected override SetDiscoveryStateRequest setRequestParameterDataToValue(byte[] parameterData)
        {
            return SetDiscoveryStateRequest.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(SetDiscoveryStateRequest discoveryState)
        {
            return discoveryState.ToPayloadData();
        }

        protected override ushort setResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(ushort value)
        {
            return Tools.ValueToData(value);
        }
        public override RequestRange<ushort> GetRequestRange(object value)
        {
            return EndpointListParameterWrapper.GetRequestRange(value);
        }
    }
}