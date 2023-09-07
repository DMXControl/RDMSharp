namespace RDMSharp.ParameterWrapper
{
    public sealed class ReleaseDHCPLeaseParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<uint>
    {
        public ReleaseDHCPLeaseParameterWrapper() : base(ERDM_Parameter.INTERFACE_RELEASE_DHCP)
        {
        }
        public override string Name => "Release DHCP Lease";
        public override string Description => "This parameter causes the device to release its DHCP address.";

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