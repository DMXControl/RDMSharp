namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class WirelessDMXParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>, IRDMManufacturerParameterWrapper
    {
        public WirelessDMXParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.WIRELESS_DMX)
        {
        }
        public override string Name => "Wireless DMX";
        public override string Description => "This parameter is used to retrieve the information, if Wireless DMX is connected.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool wirelessDMX)
        {
            return Tools.ValueToData(wirelessDMX);
        }

        protected override bool setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(bool wirelessDMX)
        {
            return Tools.ValueToData(wirelessDMX);
        }
    }
}