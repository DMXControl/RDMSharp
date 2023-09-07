namespace RDMSharp.ParameterWrapper
{
    public sealed class ManufactorLabelParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public ManufactorLabelParameterWrapper() : base(ERDM_Parameter.MANUFACTURER_LABEL)
        {
        }
        public override string Name => "Manufacturer Label";
        public override string Description => 
            "This parameter provides an ASCII text response with the Manufacturer name for the device of up " +
            "to 32 characters.";

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