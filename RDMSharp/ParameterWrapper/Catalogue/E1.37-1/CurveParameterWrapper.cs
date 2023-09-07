namespace RDMSharp.ParameterWrapper
{
    public sealed class CurveParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMCurve, byte>
    {
        public CurveParameterWrapper() : base(ERDM_Parameter.CURVE)
        {
        }
        public override string Name => "Curve";
        public override string Description => 
            "Sometimes called dimmer laws, curves set a relationship between the control level and the output level. " +
            "This is useful when matching different loads, or when matching different dimmer types. " +
            "On more advanced dimmers, it may be possible to program user-defined curves. " +
            "Transferring user defined curve data is beyond the scope of DMXControl 3.";

        protected override RDMCurve getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMCurve.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMCurve value)
        {
            return value.ToPayloadData();
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte curveId)
        {
            return Tools.ValueToData(curveId);
        }
    }
}