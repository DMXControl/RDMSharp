namespace RDMSharp.ParameterWrapper
{
    public sealed class BootSoftwareVersionIdParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<uint>
    {
        public BootSoftwareVersionIdParameterWrapper() : base(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID)
        {
        }
        public override string Name => "Boot Software Version ID";
        public override string Description => 
            "This parameter is used to retrieve the unique Boot Software Version ID for the device. The Boot " +
            "Software Version ID is a 32 - bit value determined by the Manufacturer.";

        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }
    }
}