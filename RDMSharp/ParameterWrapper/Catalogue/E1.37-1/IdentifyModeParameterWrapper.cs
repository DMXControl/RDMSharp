namespace RDMSharp.ParameterWrapper
{
    public sealed class IdentifyModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_IdentifyMode, ERDM_IdentifyMode>
    {
        public IdentifyModeParameterWrapper() : base(ERDM_Parameter.IDENTIFY_MODE)
        {
        }
        public override string Name => "Identify Mode";
        public override string Description => "This parameter allows devices to have different Identify Modes for use with the Identify Device message.";

        protected override ERDM_IdentifyMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_IdentifyMode>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_IdentifyMode identifyMode)
        {
            return Tools.ValueToData(identifyMode);
        }

        protected override ERDM_IdentifyMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_IdentifyMode>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_IdentifyMode identifyMode)
        {
            return Tools.ValueToData(identifyMode);
        }
    }
}