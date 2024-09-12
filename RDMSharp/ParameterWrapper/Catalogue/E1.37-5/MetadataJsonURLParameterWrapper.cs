namespace RDMSharp.ParameterWrapper
{
    public sealed class MetadataJsonURLParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public MetadataJsonURLParameterWrapper() : base(ERDM_Parameter.METADATA_JSON_URL)
        {
        }
        public override string Name => "Metadata JSON URL";
        public override string Description => "This parameter message returns a URL from which the JSON description of the parameter’s message structure can be retrieved from the public Internet.\nThe JSON description should be retrievable from this URL as a response to an HTTP GET request. A successful HTTP response shall contain the JSON description encoded in [UTF-8] in the body of the response. A successful HTTP response containing the JSON description should have a Content-Type of “application/schema-instance+json”. In addition to this Content-Type value, clients retrieving the JSON description shall also accept a Content-Type of “application/json”.\nTo reduce overhead on the wire, it is suggested that the URL be as short as possible.\nManufacturers should make every effort to ensure that all links that have been embedded into Responder firmware remain valid indefinitely, either directly or through the use of HTTP redirects.";

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