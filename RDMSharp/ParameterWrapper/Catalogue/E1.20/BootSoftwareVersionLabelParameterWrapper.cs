namespace RDMSharp.ParameterWrapper
{
    public sealed class BootSoftwareVersionLabelParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>
    {
        public BootSoftwareVersionLabelParameterWrapper() : base(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL)
        {
        }
        public override string Name => "Boot Software Version Label";
        public override string Description =>
            "This parameter is used to get a descriptive ASCII text label for the Boot Version of the software " +
            "for Devices that support this functionality. The label may be up to 32 characters.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }
        protected override byte[] getResponseValueToParameterData(string label)
        {
            return Tools.ValueToData(label);
        }
    }
}