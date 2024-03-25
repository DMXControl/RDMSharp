using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class OutputResponseTimeDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMOutputResponseTimeDescription>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public OutputResponseTimeDescriptionParameterWrapper() : base(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION)
        {
        }
        public override string Name => "Output Response Time Description";
        public override string Description =>
            "This parameter is used to get a descriptive ASCII text label for a response time setting. " +
            "The label may be up to 32 characters.";
        public ERDM_Parameter ValueParameterID => ERDM_Parameter.OUTPUT_RESPONSE_TIME;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.OUTPUT_RESPONSE_TIME };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;
        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte outputResponseTimeId)
        {
            return Tools.ValueToData(outputResponseTimeId);
        }

        protected override RDMOutputResponseTimeDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMOutputResponseTimeDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMOutputResponseTimeDescription value)
        {
            return value.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            if (value is RDMOutputResponseTime outputResponseTime)
                return new RequestRange<byte>(1, (byte)(outputResponseTime.Count));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}