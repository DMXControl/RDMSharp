namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512BlockAddressParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMDMXBlockAddress, ushort>
    {
        public DMX512BlockAddressParameterWrapper() : base(ERDM_Parameter.DMX_BLOCK_ADDRESS)
        {
        }
        public override string Name => "DMX512 Block Address";
        public override string Description => "This parameter provides a mechanism for block addressing the DMX512 start address of sub-devices.";

        protected override RDMDMXBlockAddress getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDMXBlockAddress.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMDMXBlockAddress value)
        {
            return value.ToPayloadData();
        }

        protected override ushort setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ushort baseDMX512Address)
        {
            return Tools.ValueToData(baseDMX512Address);
        }
    }
}