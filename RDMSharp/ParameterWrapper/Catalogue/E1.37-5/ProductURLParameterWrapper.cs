namespace RDMSharp.ParameterWrapper
{
    public sealed class ProductURLParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public ProductURLParameterWrapper() : base(ERDM_Parameter.PRODUCT_URL)
        {
        }
        public override string Name => "Product URL";
        public override string Description => "The Product URL parameter message should provide a link to access the product page on the manufacturer’s website, accessible from the public Internet. Manufacturers should make every effort to ensure this link does not change, since the message will be embedded into Responder firmware indefinitely. To reduce overhead on the wire, it is suggested that the URL be as short as possible. The URL may be used by Controllers to build user-friendly interfaces providing links to get further information about a product.";

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