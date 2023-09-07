namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class DimmingCurveParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EDimmingCurve, EDimmingCurve>, IRDMManufacturerParameterWrapper
    {
        public DimmingCurveParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.DIMMING_CURVE)
        {
        }
        public override string Name => "Dimming Curve";
        public override string Description => "This parameter is used to retrieve or set the Dimming Curve of the device. " +
            "There are two different settings. 'Linear' provides equal resolution dimming from 0-100%. " +
            "'Gamma Corrected' provides high-resolution dimming at low levels.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override EDimmingCurve getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EDimmingCurve>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(EDimmingCurve dimmingCurve)
        {
            return Tools.ValueToData(dimmingCurve);
        }

        protected override EDimmingCurve setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EDimmingCurve>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(EDimmingCurve dimmingCurve)
        {
            return Tools.ValueToData(dimmingCurve);
        }
    }
}