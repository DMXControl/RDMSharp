namespace RDMSharp.ParameterWrapper
{
    public sealed class PanInvertParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>
    {
        public PanInvertParameterWrapper() : base(ERDM_Parameter.PAN_INVERT)
        {
        }
        public override string Name => "Pan Invert";
        public override string Description => "This parameter is used to retrieve or change the Pan Invert setting.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
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