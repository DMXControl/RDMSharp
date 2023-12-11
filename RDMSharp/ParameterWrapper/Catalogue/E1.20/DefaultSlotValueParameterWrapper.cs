using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class DefaultSlotValueParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMDefaultSlotValue[]>
    {
        public DefaultSlotValueParameterWrapper() : base(ERDM_Parameter.DEFAULT_SLOT_VALUE)
        {
        }
        public override string Name => "Default Slot Value";
        public override string Description =>
            "This parameter is used for requesting the default values for the given DMX512 slot offsets " +
            "for a device. The response is a packed message containing both the slot offsets and their default " +
            "value.";

        protected override RDMDefaultSlotValue[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<RDMDefaultSlotValue> defaultSlotValue = new List<RDMDefaultSlotValue>();
            int pdl = 3;
            while (parameterData.Length >= pdl)
            {
                var bytes = parameterData.Take(pdl).ToArray();
                defaultSlotValue.Add(RDMDefaultSlotValue.FromPayloadData(bytes));
                parameterData = parameterData.Skip(pdl).ToArray();
            }
            return defaultSlotValue.ToArray();
        }

        protected override byte[] getResponseValueToParameterData(RDMDefaultSlotValue[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
                bytes.AddRange(item.ToPayloadData());

            return bytes.ToArray();
        }
    }
}