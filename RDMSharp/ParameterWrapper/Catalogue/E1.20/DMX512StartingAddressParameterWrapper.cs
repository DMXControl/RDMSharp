namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512StartingAddressParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ushort?, ushort>
    {
        public DMX512StartingAddressParameterWrapper() : base(ERDM_Parameter.DMX_START_ADDRESS)
        {
        }
        public override string Name => "DMX512 Starting Address";
        public override string Description => "This parameter is used to set or get the DMX512 start address.";

        protected override ushort? getResponseParameterDataToValue(byte[] parameterData)
        {
            ushort address = Tools.DataToUShort(ref parameterData);
            if (address == ushort.MaxValue || address == ushort.MinValue)
                return null;

            return address;
        }
        protected override byte[] getResponseValueToParameterData(ushort? address)
        {
            return Tools.ValueToData(address ?? ushort.MinValue);
        }

        protected override ushort setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ushort address)
        {
            return Tools.ValueToData(address);
        }
    }
}