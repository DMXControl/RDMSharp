namespace RDMSharp.ParameterWrapper
{
    public sealed class IPv4Address_NetmaskParameterWrapper : AbstractRDMGetParameterWrapper<uint, GetIPv4CurrentAddressResponse>
    {
        public IPv4Address_NetmaskParameterWrapper() : base(ERDM_Parameter.IPV4_CURRENT_ADDRESS)
        {
        }
        public override string Name => "IPv4 Address / Netmask";
        public override string Description => "This parameter is used to retrieve the current IPv4 Address and Netmask information for an interface.";
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

        protected override GetIPv4CurrentAddressResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetIPv4CurrentAddressResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetIPv4CurrentAddressResponse value)
        {
            return value.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}