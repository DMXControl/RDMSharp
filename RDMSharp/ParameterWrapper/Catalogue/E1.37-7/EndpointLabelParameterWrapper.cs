namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointLabelParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetSetEndpointLabel, GetSetEndpointLabel, ushort>
    {
        public EndpointLabelParameterWrapper() : base(ERDM_Parameter.ENDPOINT_LABEL)
        {
        }
        public override string Name => "Endpoint Label";
        public override string Description =>
            "This parameter provides a means of setting a descriptive label for each Endpoint on a device. " +
            "This may be used to specify the purpose of that Endpoint or the origination source for the data.";

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

        protected override GetSetEndpointLabel getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointLabel.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetEndpointLabel endpointLabel)
        {
            return endpointLabel.ToPayloadData();
        }

        protected override GetSetEndpointLabel setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointLabel.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetEndpointLabel endpointLabel)
        {
            return endpointLabel.ToPayloadData();
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