namespace RDMSharp.ParameterWrapper
{
    public sealed class HardwareAddressParameterWrapper : AbstractRDMGetParameterWrapper<uint, GetHardwareAddressResponse>
    {
        public HardwareAddressParameterWrapper() : base(ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE)
        {
        }
        public override string Name => "Hardware Address";
        public override string Description => "This parameter is used to fetch the MAC address of an interface.";
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

        protected override GetHardwareAddressResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetHardwareAddressResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetHardwareAddressResponse value)
        {
            return value.ToPayloadData();
        }
        public override RequestRange<uint> GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}