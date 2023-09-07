namespace RDMSharp.ParameterWrapper
{
    public sealed class IdentifyEndpointParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetSetIdentifyEndpoint, GetSetIdentifyEndpoint, ushort>
    {
        public IdentifyEndpointParameterWrapper() : base(ERDM_Parameter.IDENTIFY_ENDPOINT)
        {
        }
        public override string Name => "Identify Endpoint";
        public override string Description => "This parameter is used for the user to identify an Endpoint on a device. Endpoints on a device shall identify themselves if possible, using a visible and/or audible action.";

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

        protected override GetSetIdentifyEndpoint getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetIdentifyEndpoint.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetIdentifyEndpoint identifyEndpoint)
        {
            return identifyEndpoint.ToPayloadData();
        }

        protected override GetSetIdentifyEndpoint setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetIdentifyEndpoint.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetIdentifyEndpoint identifyEndpoint)
        {
            return identifyEndpoint.ToPayloadData();
        }

        protected override ushort setResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }
        public override RequestRange<ushort> GetRequestRange(object value)
        {
            return EndpointListParameterWrapper.GetRequestRange(value);
        }
    }
}