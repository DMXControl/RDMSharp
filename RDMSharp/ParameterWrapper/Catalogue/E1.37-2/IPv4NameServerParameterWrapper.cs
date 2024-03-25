namespace RDMSharp.ParameterWrapper
{
    public sealed class IPv4NameServerParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<uint, GetSetIPv4NameServer, GetSetIPv4NameServer>
    {
        public IPv4NameServerParameterWrapper() : base(ERDM_Parameter.DNS_IPV4_NAME_SERVER)
        {
        }
        public override string Name => "IPv4 Name Server";
        public override string Description => "This parameter is used to set the IPv4 DNS name servers for a device. Up to three IPv4 name servers may be configured.";
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

        protected override GetSetIPv4NameServer getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4NameServer.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetIPv4NameServer nameServer)
        {
            return nameServer.ToPayloadData();
        }

        protected override GetSetIPv4NameServer setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4NameServer.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetIPv4NameServer nameServer)
        {
            return nameServer.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}