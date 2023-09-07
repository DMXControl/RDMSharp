using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class CurveDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMCurveDescription>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public CurveDescriptionParameterWrapper() : base(ERDM_Parameter.CURVE_DESCRIPTION)
        {
        }
        public override string Name => "Curve Description";
        public override string Description => 
            "This parameter is used to get a descriptive ASCII text label for a given Curve number. " +
            "The label may be up to 32 characters.";

        public ERDM_Parameter ValueParameterID => ERDM_Parameter.CURVE;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.CURVE };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte curveId)
        {
            return Tools.ValueToData(curveId);
        }

        protected override RDMCurveDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMCurveDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMCurveDescription value)
        {
            return value.ToPayloadData();
        }

        public override RequestRange<byte> GetRequestRange(object value)
        {
            if (value is RDMCurve curve)
                return new RequestRange<byte>(1, (byte)(curve.Count));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}