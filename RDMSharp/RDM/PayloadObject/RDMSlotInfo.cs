using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMSlotInfo : AbstractRDMPayloadObject
    {
        public RDMSlotInfo(
            ushort slotOffset = default,
            ERDM_SlotType slotType = default,
            ERDM_SlotCategory slotLabelId = default)
        {
            this.SlotOffset = slotOffset;
            this.SlotType = slotType;
            this.SlotLabelId = slotLabelId;
        }

        public ushort SlotOffset { get; private set; }
        public ERDM_SlotType SlotType { get; private set; }
        public ERDM_SlotCategory SlotLabelId { get; private set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSlotInfo");
            b.AppendLine($"SlotOffset:  {SlotOffset}");
            b.AppendLine($"SlotType:    {SlotType}");
            b.AppendLine($"SlotLabelId: {SlotLabelId}");

            return b.ToString();
        }

        public static RDMSlotInfo FromPayloadData(byte[] data)
        {
            var i = new RDMSlotInfo(
                slotOffset: Tools.DataToUShort(ref data),
                slotType: Tools.DataToEnum<ERDM_SlotType>(ref data),
                slotLabelId: Tools.DataToEnum<ERDM_SlotCategory>(ref data));
            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SlotOffset));
            data.AddRange(Tools.ValueToData(this.SlotType));
            data.AddRange(Tools.ValueToData(this.SlotLabelId));
            return data.ToArray();
        }
    }
}