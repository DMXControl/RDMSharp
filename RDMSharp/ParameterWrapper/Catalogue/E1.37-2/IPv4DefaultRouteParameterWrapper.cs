namespace RDMSharp.ParameterWrapper
{
    public sealed class IPv4DefaultRouteParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<uint, GetSetIPv4DefaultRoute, GetSetIPv4DefaultRoute>
    {
        public IPv4DefaultRouteParameterWrapper() : base(ERDM_Parameter.IPV4_DEFAULT_ROUTE)
        {
        }
        public override string Name => "IPv4 Default Route";
        public override string Description =>
            "This parameter is used to get and set the default IPv4 route for a device. The default route is a global property of a device. " +
            "If a default route exists, it may point to a gateway or, in the case of a point - to - point link, to an " +
            "interface without a gateway. If a gateway is used, the device may be able to provide the " +
            "interface the gateway is reachable on. This is extra information which may facilitate debugging, " +
            "but devices are not required to provide it.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.LIST_INTERFACES };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override uint getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(uint interfaceId)
        {
            return Tools.ValueToData(interfaceId);
        }

        protected override GetSetIPv4DefaultRoute getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4DefaultRoute.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetIPv4DefaultRoute defaultRoute)
        {
            return defaultRoute.ToPayloadData();
        }

        protected override GetSetIPv4DefaultRoute setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4DefaultRoute.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetIPv4DefaultRoute defaultRoute)
        {
            return defaultRoute.ToPayloadData();
        }
        public override RequestRange<uint> GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}