namespace RDMSharp.ParameterWrapper
{
    public sealed class PanTiltSwapParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>
    {
        public PanTiltSwapParameterWrapper() : base(ERDM_Parameter.PAN_TILT_SWAP)
        {
        }
        public override string Name => "Pan/Tilt Swap";
        public override string Description => "This parameter is used to retrieve or change the Pan/Tilt Swap setting.";

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