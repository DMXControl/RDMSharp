namespace RDMSharp.ParameterWrapper
{
    public sealed class LampHoursParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<uint, uint>
    {
        public LampHoursParameterWrapper() : base(ERDM_Parameter.LAMP_HOURS)
        {
        }
        public override string Name => "Lamp Hours";
        public override string Description => "This parameter is used to retrieve the number of lamp hours or to set the counter in the device to a specific starting value.";

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

        protected override byte[] setRequestValueToParameterData(uint deviceHours)
        {
            return Tools.ValueToData(deviceHours);
        }
    }
}