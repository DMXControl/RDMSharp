namespace RDMSharp.ParameterWrapper
{
    public sealed class DisplayLevelParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<byte, byte>
    {
        public DisplayLevelParameterWrapper() : base(ERDM_Parameter.DISPLAY_LEVEL)
        {
        }
        public override string Name => "Display Level";
        public override string Description => "This parameter is used to retrieve or change the Display Intensity setting.";

        protected override byte getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(byte value)
        {
            return Tools.ValueToData(value);
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte displayLevel)
        {
            return Tools.ValueToData(displayLevel);
        }
    }
}