using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMDefaultSlotValue : AbstractRDMPayloadObject
    {
        public RDMDefaultSlotValue(
            ushort slotOffset = 0,
            byte defaultSlotValue = 0)
        {
            this.SlotOffset = slotOffset;
            this.DefaultSlotValue = defaultSlotValue;
        }

        public ushort SlotOffset { get; private set; }
        public byte DefaultSlotValue { get; private set; }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMDefaultSlotValue");
            b.AppendLine($"SlotOffset:       {SlotOffset}");
            b.AppendLine($"DefaultSlotValue: {DefaultSlotValue}");

            return b.ToString();
        }

        public static RDMDefaultSlotValue FromPayloadData(byte[] data)
        {
            var i = new RDMDefaultSlotValue(
                slotOffset: Tools.DataToUShort(ref data),
                defaultSlotValue: Tools.DataToByte(ref data));
            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SlotOffset));
            data.AddRange(Tools.ValueToData(this.DefaultSlotValue));
            return data.ToArray();
        }
    }
}