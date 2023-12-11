namespace RDMSharp.ParameterWrapper
{
    public sealed class MaximumLevelParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ushort, ushort>
    {
        public MaximumLevelParameterWrapper() : base(ERDM_Parameter.MAXIMUM_LEVEL)
        {
        }
        public override string Name => "Maximum Level";
        public override string Description =>
            "Maximum Level sets the highest level that the output may go to in response to the control signal - DMX512, Preset Playback or otherwise. " +
            "This can be used to provide Topset functionality.";

        protected override ushort getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ushort maximumLevel)
        {
            return Tools.ValueToData(maximumLevel);
        }

        protected override ushort setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ushort maximumLevel)
        {
            return Tools.ValueToData(maximumLevel);
        }
    }
}