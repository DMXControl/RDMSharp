using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DEFAULT_SLOT_VALUE, Command.ECommandDublicte.GetResponse, true, "slots")]
    public class RDMDefaultSlotValue : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMDefaultSlotValue(
            [DataTreeObjectParameter("id")] ushort slotOffset = 0,
            [DataTreeObjectParameter("default_value")] byte defaultSlotValue = 0)
        {
            this.SlotOffset = slotOffset;
            this.DefaultSlotValue = defaultSlotValue;
        }

        [DataTreeObjectProperty("id", 0)]
        public ushort SlotOffset { get; private set; }
        [DataTreeObjectProperty("default_value", 1)]
        public byte DefaultSlotValue { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMDefaultSlotValue");
            b.AppendLine($"SlotOffset:       {SlotOffset}");
            b.AppendLine($"DefaultSlotValue: {DefaultSlotValue}");

            return b.ToString();
        }

        public static RDMDefaultSlotValue FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DEFAULT_SLOT_VALUE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDefaultSlotValue FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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