using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMSlotDescription : AbstractRDMPayloadObject
    {
        public RDMSlotDescription(
            ushort slotId = 0,
            string description = "")
        {
            this.SlotId = slotId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public ushort SlotId { get; private set; }
        public string Description { get; private set; }
        public const int PDL_MIN = 2;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"RDMSlotDescription: {SlotId} - {Description}";
        }

        public static RDMSlotDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SLOT_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSlotDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMSlotDescription(
                slotId: Tools.DataToUShort(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SlotId));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
