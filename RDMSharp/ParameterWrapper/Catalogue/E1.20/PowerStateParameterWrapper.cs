namespace RDMSharp.ParameterWrapper
{
    public sealed class PowerStateParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_PowerState, ERDM_PowerState>
    {
        public PowerStateParameterWrapper() : base(ERDM_Parameter.POWER_STATE)
        {
        }
        public override string Name => "Power State";
        public override string Description => "This parameter is used to retrieve or change the current device Power State. Power State specifies the current operating mode of the device.";

        protected override ERDM_PowerState getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_PowerState>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_PowerState value)
        {
            return Tools.ValueToData(value);
        }

        protected override ERDM_PowerState setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_PowerState>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_PowerState powerState)
        {
            return Tools.ValueToData(powerState);
        }
    }
}