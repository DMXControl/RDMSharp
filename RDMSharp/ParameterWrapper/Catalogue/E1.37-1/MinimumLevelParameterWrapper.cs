namespace RDMSharp.ParameterWrapper
{
    public sealed class MinimumLevelParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMMinimumLevel, RDMMinimumLevel>
    {
        public MinimumLevelParameterWrapper() : base(ERDM_Parameter.MINIMUM_LEVEL)
        {
        }
        public override string Name => "Minimum Level";
        public override string Description => 
            "Minimum Level sets the lowest level that the output may go to in response to the control signal - DMX512, Preset Playback or otherwise. " +
            "By setting the On Below Minimum value, this can be used to provide Preheat functionality for incandescent lamps.";

        protected override RDMMinimumLevel getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMMinimumLevel.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMMinimumLevel minimumLevel)
        {
            return Tools.ValueToData(minimumLevel);
        }

        protected override RDMMinimumLevel setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMMinimumLevel.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMMinimumLevel minimumLevel)
        {
            return Tools.ValueToData(minimumLevel);
        }
    }
}