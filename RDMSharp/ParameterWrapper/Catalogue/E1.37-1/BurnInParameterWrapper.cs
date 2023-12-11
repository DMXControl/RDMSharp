namespace RDMSharp.ParameterWrapper
{
    public sealed class BurnInParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<byte, byte>
    {
        public BurnInParameterWrapper() : base(ERDM_Parameter.BURN_IN)
        {
        }
        public override string Name => "Burn-In";
        public override string Description =>
            "This parameter provides a mechanism for devices that require specified burn-in times.\r\n\r\n" +
            "In order for fluorescent lamps to operate properly with all types of fluorescent dimming ballasts they must be operated continuously " +
            "at full output for a manufacturer recommended period of time (BURN_IN).\r\n\r\n" +
            "This parameter allows users to set a burn-in time for dimmers " +
            "controlling fluorescent ballasts after changing lamps.";

        protected override byte getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(byte hours)
        {
            return Tools.ValueToData(hours);
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte hours)
        {
            return Tools.ValueToData(hours);
        }
    }
}