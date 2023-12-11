namespace RDMSharp.ParameterWrapper
{
    public sealed class DevicePowerCyclesParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<uint, uint>
    {
        public DevicePowerCyclesParameterWrapper() : base(ERDM_Parameter.DEVICE_POWER_CYCLES)
        {
        }
        public override string Name => "Device Power Cycles";
        public override string Description =>
            "This parameter is used to retrieve or set the number of Power-up cycles for the device. Some " +
            "devices may only support the retrieval of this parameter and not allow the device’s " +
            "power - up cycles to be set.";

        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }

        protected override uint setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(uint powerCyclesCount)
        {
            return Tools.ValueToData(powerCyclesCount);
        }
    }
}