namespace RDMSharp.ParameterWrapper
{
    public sealed class SoftwareVersionLabelParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>
    {
        public SoftwareVersionLabelParameterWrapper() : base(ERDM_Parameter.SOFTWARE_VERSION_LABEL)
        {
        }
        public override string Name => "Software Version Label";
        public override string Description => 
            "This parameter is used to get a descriptive ASCII text label for the device’s operating software " +
            "version. The label may be up to 32 characters.";

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