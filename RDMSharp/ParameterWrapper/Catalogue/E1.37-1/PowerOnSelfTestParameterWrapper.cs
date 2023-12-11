namespace RDMSharp.ParameterWrapper
{
    public sealed class PowerOnSelfTestParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>
    {
        public PowerOnSelfTestParameterWrapper() : base(ERDM_Parameter.POWER_ON_SELF_TEST)
        {
        }
        public override string Name => "Power-On Self Test";
        public override string Description =>
            "This parameter is used to get or set the Power-On Self Test mode parameter. " +
            "This allows devices to enable or disable a power-on self test mode that executes automatically on power up.";
        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool powerOnSelfTest)
        {
            return Tools.ValueToData(powerOnSelfTest);
        }

        protected override bool setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(bool powerOnSelfTest)
        {
            return Tools.ValueToData(powerOnSelfTest);
        }
    }
}