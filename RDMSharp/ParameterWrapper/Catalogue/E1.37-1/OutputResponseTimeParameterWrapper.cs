namespace RDMSharp.ParameterWrapper
{
    public sealed class OutputResponseTimeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMOutputResponseTime, byte>
    {
        public OutputResponseTimeParameterWrapper() : base(ERDM_Parameter.OUTPUT_RESPONSE_TIME)
        {
        }
        public override string Name => "Output Response Time";
        public override string Description =>
            "Dimmers often have a variable response time that smoothes fades that might otherwise exhibit a stepping behavior between levels. " +
            "The consequence of smoothing fades using this method is that the dimmer may not turn on or off as quickly as it would without the slowed response. " +
            "Dimmers with variable response times allow the user to achieve a balance between speed and smoothness in fades.";

        protected override RDMOutputResponseTime getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMOutputResponseTime.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMOutputResponseTime value)
        {
            return value.ToPayloadData();
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte outputResponseTimeId)
        {
            return Tools.ValueToData(outputResponseTimeId);
        }
    }
}