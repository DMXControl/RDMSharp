namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceLabelParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<string, string>
    {
        public DeviceLabelParameterWrapper() : base(ERDM_Parameter.DEVICE_LABEL)
        {
        }
        public override string Name => "Device Label";
        public override string Description => 
            "This parameter provides a means of setting a descriptive label for each device. This may be used" +
            "for identifying a dimmer rack number or specifying the device’s location.";
        
        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }
        protected override byte[] getResponseValueToParameterData(string label)
        {
            return Tools.ValueToData(label);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }
    }
}