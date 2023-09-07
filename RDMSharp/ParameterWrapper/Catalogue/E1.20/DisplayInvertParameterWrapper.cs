namespace RDMSharp.ParameterWrapper
{
    public sealed class DisplayInvertParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_DisplayInvert, ERDM_DisplayInvert>
    {
        public DisplayInvertParameterWrapper() : base(ERDM_Parameter.DISPLAY_INVERT)
        {
        }
        public override string Name => "Display Invert";
        public override string Description => "This parameter is used to retrieve or change the Display Invert setting. Invert is often used to rotate the display image by 180 degrees.";

        protected override ERDM_DisplayInvert getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_DisplayInvert>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_DisplayInvert value)
        {
            return Tools.ValueToData(value);
        }

        protected override ERDM_DisplayInvert setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_DisplayInvert>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_DisplayInvert displayInvert)
        {
            return Tools.ValueToData(displayInvert);
        }
    }
}