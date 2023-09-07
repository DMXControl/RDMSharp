namespace RDMSharp.ParameterWrapper
{
    public sealed class LampOnModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_LampMode, ERDM_LampMode>
    {
        public LampOnModeParameterWrapper() : base(ERDM_Parameter.LAMP_ON_MODE)
        {
        }
        public override string Name => "Lamp On Mode";
        public override string Description => "This parameter is used to retrieve or change the current Lamp On Mode. Lamp On Mode defines the conditions under which a lamp will be struck.";

        protected override ERDM_LampMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_LampMode>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_LampMode value)
        {
            return Tools.ValueToData(value);
        }

        protected override ERDM_LampMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_LampMode>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_LampMode lampOnMode)
        {
            return Tools.ValueToData(lampOnMode);
        }
    }
}