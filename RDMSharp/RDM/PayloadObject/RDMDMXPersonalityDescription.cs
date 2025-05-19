using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, Command.ECommandDublicte.GetResponse)]
    public class RDMDMXPersonalityDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        [DataTreeObjectConstructor]
        public RDMDMXPersonalityDescription(
            [DataTreeObjectParameter("personality")] byte personalityId = 1,
            [DataTreeObjectParameter("dmx_slots_required")] ushort slots = 0,
            [DataTreeObjectParameter("description")] string description = "")
        {
            this.PersonalityId = personalityId;
            this.Slots = slots;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }


        [DataTreeObjectProperty("personality", 0)]
        public byte PersonalityId { get; private set; }

        [DataTreeObjectDependecieProperty("slot", ERDM_Parameter.SLOT_DESCRIPTION, Command.ECommandDublicte.GetRequest)]
        [DataTreeObjectProperty("dmx_slots_required", 1)]
        public ushort Slots { get; private set; }

        [DataTreeObjectProperty("description", 2)]
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => PersonalityId;

        public const int PDL_MIN = 3;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"{PersonalityId} ({Slots}) - {Description}";
        }

        public static RDMDMXPersonalityDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMXPersonalityDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMDMXPersonalityDescription(
                personalityId: Tools.DataToByte(ref data),
                slots: Tools.DataToUShort(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.PersonalityId));
            data.AddRange(Tools.ValueToData(this.Slots));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
