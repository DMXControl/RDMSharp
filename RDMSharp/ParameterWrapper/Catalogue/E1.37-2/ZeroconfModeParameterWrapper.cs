namespace RDMSharp.ParameterWrapper
{
    public sealed class ZeroconfModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<uint, GetSetIPV4_xxx_Mode, GetSetIPV4_xxx_Mode>
    {
        public ZeroconfModeParameterWrapper() : base(ERDM_Parameter.IPV4_ZEROCONF_MODE)
        {
        }
        public override string Name => "Zeroconf Mode";
        public override string Description => 
            "This parameter is used to retrieve or change the IPv4 Link Local Addressing (Zeroconf) [IPv4LL] " +
            "mode on an interface. Changes to the Zeroconf mode will not take effect until the " +
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

        protected override GetSetIPV4_xxx_Mode getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPV4_xxx_Mode.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetIPV4_xxx_Mode dhcpMode)
        {
            return dhcpMode.ToPayloadData();
        }

        protected override GetSetIPV4_xxx_Mode setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetIPV4_xxx_Mode.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetIPV4_xxx_Mode dhcpMode)
        {
            return dhcpMode.ToPayloadData();
        }
        public override RequestRange<uint> GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}