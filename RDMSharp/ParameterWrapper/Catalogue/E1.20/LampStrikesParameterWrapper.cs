namespace RDMSharp.ParameterWrapper
{
    public sealed class LampStrikesParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<uint, uint>
    {
        public LampStrikesParameterWrapper() : base(ERDM_Parameter.LAMP_STRIKES)
        {
        }
        public override string Name => "Lamp Strikes";
        public override string Description => "This parameter is used to retrieve the number of lamp strikes or to set the counter in the device to a specific starting value.";

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