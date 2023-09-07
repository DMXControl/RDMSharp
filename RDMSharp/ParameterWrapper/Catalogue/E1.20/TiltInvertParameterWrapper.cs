namespace RDMSharp.ParameterWrapper
{
    public sealed class TiltInvertParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>
    {
        public TiltInvertParameterWrapper() : base(ERDM_Parameter.TILT_INVERT)
        {
        }
        public override string Name => "Tilt Invert";
        public override string Description => "This parameter is used to retrieve or change the Tilt Invert setting.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool panInvert)
        {
            return Tools.ValueToData(panInvert);
        }

        protected override bool setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(bool panInvert)
        {
            return Tools.ValueToData(panInvert);
        }
    }
}