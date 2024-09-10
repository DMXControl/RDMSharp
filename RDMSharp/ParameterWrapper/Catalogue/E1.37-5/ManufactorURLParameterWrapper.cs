namespace RDMSharp.ParameterWrapper
{
    public sealed class ManufactorURLParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public ManufactorURLParameterWrapper() : base(ERDM_Parameter.MANUFACTURER_URL)
        {
        }
        public override string Name => "Manufacturer URL";
        public override string Description => "The Manufacturer URL parameter message should provide a URL to the manufacturer’s website, accessible from the public Internet. This URL may be used by Controllers to build user-friendly interfaces that provide links, as necessary, to get further information about a product.";

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