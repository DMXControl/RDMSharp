namespace RDMSharp.ParameterWrapper
{
    public sealed class FirmwareURLParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public FirmwareURLParameterWrapper() : base(ERDM_Parameter.FIRMWARE_URL)
        {
        }
        public override string Name => "Firmware URL";
        public override string Description => "The Firmware URL parameter message should provide a link to access the product firmware on a manufacturer’s website, accessible from the public Internet. Manufacturers should make every effort to ensure this link does not change, since the message will be embedded into Responder firmware indefinitely. To reduce overhead on the wire, it is suggested that the URL be as short as possible.";

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