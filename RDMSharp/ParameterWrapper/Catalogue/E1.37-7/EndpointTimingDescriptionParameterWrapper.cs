using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointTimingDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, GetEndpointTimingDescriptionResponse>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public EndpointTimingDescriptionParameterWrapper() : base(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION)
        {
        }
        public override string Name => "Endpoint Timing Description";
        public override string Description => 
            "This parameter is used to get a descriptive ASCII text label for a given Endpoint timing setting. " +
            "The label may be up to 32 ASCII characters.";
        public ERDM_Parameter ValueParameterID => ERDM_Parameter.ENDPOINT_TIMING;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_TIMING };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte timingId)
        {
            return Tools.ValueToData(timingId);
        }

        protected override GetEndpointTimingDescriptionResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetEndpointTimingDescriptionResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetEndpointTimingDescriptionResponse value)
        {
            return value.ToPayloadData();
        }

        public override RequestRange<byte> GetRequestRange(object value)
        {
            if (value is GetEndpointTimingResponse getEndpointTiming)
                return new RequestRange<byte>(1, (byte)(getEndpointTiming.Count));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}