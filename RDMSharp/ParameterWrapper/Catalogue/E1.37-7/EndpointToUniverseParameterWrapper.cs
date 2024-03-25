namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointToUniverseParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetSetEndpointToUniverse, GetSetEndpointToUniverse, ushort>
    {
        public EndpointToUniverseParameterWrapper() : base(ERDM_Parameter.ENDPOINT_TO_UNIVERSE)
        {
        }
        public override string Name => "Endpoint to Universe";
        public override string Description =>
            "This parameter is used to assign an Endpoint on a device to a specific sACN DMX512 Universe. " +
            "It may also be used within a Splitter to assign inputs to outputs.";

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

        protected override GetSetEndpointToUniverse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointToUniverse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetEndpointToUniverse endpointToUniverse)
        {
            return endpointToUniverse.ToPayloadData();
        }

        protected override GetSetEndpointToUniverse setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointToUniverse.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetEndpointToUniverse endpointToUniverse)
        {
            return endpointToUniverse.ToPayloadData();
        }

        protected override ushort setResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return EndpointListParameterWrapper.GetRequestRange(value);
        }
    }
}