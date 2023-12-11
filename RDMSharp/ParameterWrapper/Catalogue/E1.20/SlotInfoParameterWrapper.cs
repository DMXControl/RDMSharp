using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SlotInfoParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMSlotInfo[]>
    {
        public SlotInfoParameterWrapper() : base(ERDM_Parameter.SLOT_INFO)
        {
        }
        public override string Name => "Slot Info";
        public override string Description =>
            "This parameter is used to retrieve basic information about the functionality of the DMX512 slots " +
            "used to control the device. The response is a packed list of Slot Label ID’s referencing into the " +
            "Slot Labels table.";

        protected override RDMSlotInfo[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<RDMSlotInfo> slotInfos = new List<RDMSlotInfo>();
            int pdl = 5;
            while (parameterData.Length >= pdl)
            {
                var bytes = parameterData.Take(pdl).ToArray();
                slotInfos.Add(RDMSlotInfo.FromPayloadData(bytes));
                parameterData = parameterData.Skip(pdl).ToArray();
            }
            return slotInfos.ToArray();
        }

        protected override byte[] getResponseValueToParameterData(RDMSlotInfo[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
                bytes.AddRange(item.ToPayloadData());

            return bytes.ToArray();
        }
    }
}