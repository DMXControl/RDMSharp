namespace RDMSharp.ParameterWrapper
{
    public sealed class RenewDHCPLeaseParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<uint>
    {
        public RenewDHCPLeaseParameterWrapper() : base(ERDM_Parameter.INTERFACE_RENEW_DHCP)
        {
        }
        public override string Name => "Renew DHCP Lease";
        public override string Description => "This parameter causes the device to attempt to renew its DHCP address.";

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