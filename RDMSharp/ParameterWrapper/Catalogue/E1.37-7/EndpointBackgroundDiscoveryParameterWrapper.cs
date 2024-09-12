namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointBackgroundDiscoveryParameterWrapper : AbstractRDMGetSetParameterWrapperRanged<ushort, GetSetEndpointBackgroundDiscovery, GetSetEndpointBackgroundDiscovery, ushort>
    {
        public EndpointBackgroundDiscoveryParameterWrapper() : base(ERDM_Parameter.BACKGROUND_DISCOVERY)
        {
        }
        public override string Name => "Background Discovery";
        public override string Description => "This parameter is used to enable/disable background E1.20 RDM discovery for an Endpoint. " +
            "Background Discovery is an ongoing autonomous discovery routine running on a device that " +
            "periodically discovers additional RDM Responders as they come online.\r\n\r\n" +
            "The background discovery process may also include the periodic unmuting of all RDM " +
            "Responders and re - muting all known RDM Responders before performing an incremental " +
            "discovery to ensure all connected RDM Responders are properly discovered regardless of their " +
            "previous connection state. The frequency that Background Discovery operates at is up to the " +
            "implementer and may vary based on the amount of other traffic.";

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

        protected override GetSetEndpointBackgroundDiscovery getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointBackgroundDiscovery.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetEndpointBackgroundDiscovery backgroundDiscovery)
        {
            return backgroundDiscovery.ToPayloadData();
        }

        protected override GetSetEndpointBackgroundDiscovery setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetEndpointBackgroundDiscovery.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetEndpointBackgroundDiscovery backgroundDiscovery)
        {
            return backgroundDiscovery.ToPayloadData();
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