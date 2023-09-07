using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class ListInterfacesParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<GetInterfaceListResponse>, IRDMBlueprintParameterWrapper
    {
        public ListInterfacesParameterWrapper() : base(ERDM_Parameter.LIST_INTERFACES)
        {
        }
        public override string Name => "Interface List";
        public override string Description => "This parameter returns a packed list of network interface descriptors, representing the IPv4 network interfaces on the device.";

        protected override GetInterfaceListResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetInterfaceListResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetInterfaceListResponse value)
        {
            return value.ToPayloadData();
        }
        internal static RequestRange<uint> GetRequestRange(object value)
        {
            if (value == null)
                return new RequestRange<uint>(0x00000001, 0xFFFFFF00);

            if (value is GetInterfaceListResponse getInterfaceListResponse)
                value = getInterfaceListResponse.Interfaces.ToList();

            uint max = 0;
            if (value is IEnumerable<object> @enumerable)
            {
                var interfaceDescriptors = enumerable.OfType<InterfaceDescriptor>();
                if (interfaceDescriptors.Count() != 0)
                    max = interfaceDescriptors.Max(i => i.InterfaceId);

                return new RequestRange<uint>(0, max);
            }

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}