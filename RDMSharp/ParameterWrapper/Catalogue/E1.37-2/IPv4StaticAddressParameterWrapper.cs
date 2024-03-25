namespace RDMSharp.ParameterWrapper
{
    public sealed class IPv4StaticAddressParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<uint, GetSetIPv4StaticAddress, GetSetIPv4StaticAddress>
    {
        public IPv4StaticAddressParameterWrapper() : base(ERDM_Parameter.IPV4_STATIC_ADDRESS)
        {
        }
        public override string Name => "IPv4 Static Address";
        public override string Description =>
            "This parameter is used to statically configure the IPv4 address and network mask on an " +
            "interface. Changes to the IPv4 Static Address will not take effect until the " +
            "Interface Apply Configuration message is received by the interface.";
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

        protected override GetSetIPv4StaticAddress getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4StaticAddress.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetIPv4StaticAddress staticAddress)
        {
            return staticAddress.ToPayloadData();
        }

        protected override GetSetIPv4StaticAddress setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPv4StaticAddress.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetIPv4StaticAddress staticAddress)
        {
            return staticAddress.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}