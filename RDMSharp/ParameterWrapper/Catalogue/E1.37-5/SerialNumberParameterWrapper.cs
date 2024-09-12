namespace RDMSharp.ParameterWrapper
{
    public sealed class SerialNumberParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<string>
    {
        public SerialNumberParameterWrapper() : base(ERDM_Parameter.SERIAL_NUMBER)
        {
        }
        public override string Name => "Serial Number";
        public override string Description => "This parameter is used to obtain a text string that contains the manufacturer’s serial number for the device.";

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