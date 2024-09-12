namespace RDMSharp.ParameterWrapper
{
    public sealed class IdentifyTimeoutParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<short, short>
    {
        public IdentifyTimeoutParameterWrapper() : base(ERDM_Parameter.IDENTIFY_TIMEOUT)
        {
        }
        public override string Name => "Identify Timeout";
        public override string Description => "This parameter is an extension to the IDENTIFY_DEVICE command in Section 10.11.1 of [RDM]. If this message is supported, then it extends the IDENTIFY_DEVICE function to have a specified timeout before the Identify mode stops.";

        protected override short getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToShort(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(short value)
        {
            return Tools.ValueToData(value);
        }

        protected override short setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(short state)
        {
            return Tools.ValueToData(state);
        }
    }
}