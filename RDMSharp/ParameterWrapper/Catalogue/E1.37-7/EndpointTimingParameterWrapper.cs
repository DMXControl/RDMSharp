using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointTimingParameterWrapper : AbstractRDMGetSetParameterWrapper<ushort, GetEndpointTimingResponse, SetEndpointTimingRequest, ushort>
    {
        public EndpointTimingParameterWrapper() : base(ERDM_Parameter.ENDPOINT_TIMING)
        {
        }
        public override string Name => "Endpoint Timing";
        public override string Description =>
            "This parameter is used to get and set the timing profile on Endpoints that support selecting " +
            "different timing and refresh profiles.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_LIST };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }

        protected override GetEndpointTimingResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetEndpointTimingResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetEndpointTimingResponse timingRequest)
        {
            return timingRequest.ToPayloadData();
        }

        protected override SetEndpointTimingRequest setRequestParameterDataToValue(byte[] parameterData)
        {
            return SetEndpointTimingRequest.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(SetEndpointTimingRequest timingRequest)
        {
            return timingRequest.ToPayloadData();
        }

        protected override ushort setResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }
        public override RequestRange<ushort> GetRequestRange(object value)
        {
            if (value == null)
                return new RequestRange<ushort>(0x0001, 0xF9FF);

            ushort max = 0;
            if (value is IEnumerable<object> @enumerable)
            {
                var endpointDescriptors = enumerable.OfType<EndpointDescriptor>();
                if (endpointDescriptors.Count() != 0)
                    max = endpointDescriptors.Max(i => i.EndpointId);

                return new RequestRange<ushort>(0, max);
            }

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}