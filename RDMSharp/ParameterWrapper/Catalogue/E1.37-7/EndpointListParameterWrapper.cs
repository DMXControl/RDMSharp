using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class EndpointListParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<GetEndpointListResponse>, IRDMBlueprintParameterWrapper
    {
        public EndpointListParameterWrapper() : base(ERDM_Parameter.ENDPOINT_LIST)
        {
        }
        public override string Name => "Endpoint List";
        public override string Description => "This parameter is used to retrieve a packed list of all Endpoints that exist on a device.";

        protected override GetEndpointListResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetEndpointListResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetEndpointListResponse value)
        {
            return value.ToPayloadData();
        }
        internal static RequestRange<ushort> GetRequestRange(object value)
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