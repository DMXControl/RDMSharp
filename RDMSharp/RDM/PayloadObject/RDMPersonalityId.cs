using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DMX_PERSONALITY_ID, Command.ECommandDublicate.GetResponse)]
    public class RDMPersonalityId : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        [DataTreeObjectConstructor]
        public RDMPersonalityId(
            [DataTreeObjectParameter("personality")] byte personalityId = 0,
            [DataTreeObjectParameter("major_id")] ushort majorPersonalityId = 0,
            [DataTreeObjectParameter("minor_id")] ushort minorPersonalityId = 0)
        {
            this.PersonalityId = personalityId;
            this.MinorPersonalityId = majorPersonalityId;
            this.MinorPersonalityId = minorPersonalityId;
        }

        public byte PersonalityId { get; private set; }
        public ushort MajorPersonalityId { get; private set; }
        public ushort MinorPersonalityId { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => PersonalityId;

        public const int PDL = 5;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMPersonalityId");
            b.AppendLine($"PersonalityId:      {PersonalityId}");
            b.AppendLine($"MajorPersonalityId: 0x{MajorPersonalityId:X4}");
            b.AppendLine($"MinorPersonalityId: 0x{MinorPersonalityId:X4}");

            return b.ToString();
        }

        public static RDMPersonalityId FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY_ID, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPersonalityId FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var personalityId = Tools.DataToByte(ref data);
            var majorPersonalityId = Tools.DataToUShort(ref data);
            var minorPersonalityId = Tools.DataToUShort(ref data);

            var i = new RDMPersonalityId(
                personalityId: personalityId,
                majorPersonalityId: majorPersonalityId,
                minorPersonalityId: minorPersonalityId
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.PersonalityId));
            data.AddRange(Tools.ValueToData(this.MajorPersonalityId));
            data.AddRange(Tools.ValueToData(this.MinorPersonalityId));
            return data.ToArray();
        }
    }
}
