namespace RDMSharp.ParameterWrapper
{
    public sealed class LampStateParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_LampState, ERDM_LampState>
    {
        public LampStateParameterWrapper() : base(ERDM_Parameter.LAMP_STATE)
        {
        }
        public override string Name => "Lamp State";
        public override string Description => "This parameter is used to retrieve or change the current operating state of the lamp.";

        protected override ERDM_LampState getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_LampState>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_LampState value)
        {
            return Tools.ValueToData(value);
        }

        protected override ERDM_LampState setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_LampState>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_LampState lampState)
        {
            return Tools.ValueToData(lampState);
        }
    }
}