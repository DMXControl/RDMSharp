namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceModelDescriptionParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>, IRDMBlueprintParameterWrapper
    {
        public DeviceModelDescriptionParameterWrapper() : base(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION)
        {
        }
        public override string Name => "Device Model Description";
        public override string Description => "This parameter provides a text description of up to 32 characters for the device model type.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }
    }
}