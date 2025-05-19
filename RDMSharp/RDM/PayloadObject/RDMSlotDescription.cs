using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.SLOT_DESCRIPTION, Command.ECommandDublicte.GetResponse)]
    public class RDMSlotDescription : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMSlotDescription(
            [DataTreeObjectParameter("slot")] ushort slotId = 0,
            [DataTreeObjectParameter("description")] string description = "")
        {
            this.SlotId = slotId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        [DataTreeObjectProperty("slot", 0)]
        public ushort SlotId { get; private set; }

        [DataTreeObjectProperty("description", 1)]
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
