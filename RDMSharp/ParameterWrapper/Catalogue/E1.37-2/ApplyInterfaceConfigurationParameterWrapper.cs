namespace RDMSharp.ParameterWrapper
{
    public sealed class ApplyInterfaceConfigurationParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<uint>
    {
        public ApplyInterfaceConfigurationParameterWrapper() : base(ERDM_Parameter.INTERFACE_APPLY_CONFIGURATION)
        {
        }
        public override string Name => "Apply Interface Configuration";
        public override string Description => 
            "This parameter applies the stored configuration to an interface. The configuration of an interface " +
            "shall include, but is not limited to, the settings in IPV4 Static Address, " +
            "IPV4 DHCP Mode(if supported) and IPV4 Zeroconf Mode(if supported).";

        protected override uint setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(uint interfaceId)
        {
            return Tools.ValueToData(interfaceId);
        }
    }
}