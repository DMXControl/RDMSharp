namespace RDMSharp.ParameterWrapper
{
    public sealed class PerformSelfTestParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, byte>
    {
        public PerformSelfTestParameterWrapper() : base(ERDM_Parameter.PERFORM_SELFTEST)
        {
        }
        public override string Name => "Perform Self Test";
        public override string Description => 
            "This message is used to execute any built in Self-Test routine that may " +
            "be present. The test may run continuously until receiving a message with a " +
            "SELF_TEST_OFF value, or may exit upon completion.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte selfTestId)
        {
            return Tools.ValueToData(selfTestId);
        }
    }
}