namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointRDMTrafficEnableParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetSetEndpointRDMTrafficEnable, GetSetEndpointRDMTrafficEnable, ushort>
    {
        public EndpointRDMTrafficEnableParameterWrapper() : base(ERDM_Parameter.RDM_TRAFFIC_ENABLE)
        {
        }
        public override string Name => "Enable RDM Traffic on Endpoint";
        public override string Description => 
            "This parameter is used to enable or disable the RDM Traffic on a specified Endpoint. This shall " +
            "only affect traffic using the defined RDM Start Code and will not affect the traffic for normal " +
            "DMX packets or any other alternate packet types.";

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

        protected override GetSetEndpointRDMTrafficEnable getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointRDMTrafficEnable.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetEndpointRDMTrafficEnable rdmTrafficEnable)
        {
            return rdmTrafficEnable.ToPayloadData();
        }

        protected override GetSetEndpointRDMTrafficEnable setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointRDMTrafficEnable.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetEndpointRDMTrafficEnable rdmTrafficEnable)
        {
            return rdmTrafficEnable.ToPayloadData();
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