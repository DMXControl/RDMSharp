namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class SerialNumberParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMManufacturerParameterWrapper
    {
        public SerialNumberParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.SERIAL_NUMBER)
        {
        }
        public override string Name => "Serial Number";
        public override string Description => "This parameter is used to retrieve the Serial Number of the device.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }
        protected override byte[] getResponseValueToParameterData(string serialNumber)
        {
            return Tools.ValueToData(serialNumber);
        }
    }
}