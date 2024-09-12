namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointModeParameterWrapper : AbstractRDMGetSetParameterWrapperRanged<ushort, GetSetEndpointMode, GetSetEndpointMode, ushort>
    {
        public EndpointModeParameterWrapper() : base(ERDM_Parameter.ENDPOINT_MODE)
        {
        }
        public override string Name => "Endpoint Mode";
        public override string Description => "This parameter is used to specify whether an Endpoint operates in Input, Output, or Disabled mode.";

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

        protected override GetSetEndpointMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointMode.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetEndpointMode endpointMode)
        {
            return endpointMode.ToPayloadData();
        }

        protected override GetSetEndpointMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointMode.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetEndpointMode endpointMode)
        {
            return endpointMode.ToPayloadData();
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