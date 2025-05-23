using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.LOCK_STATE_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class RDMLockStateDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        [DataTreeObjectConstructor]
        public RDMLockStateDescription(
            [DataTreeObjectParameter("state")] byte lockStateId = 1,
            [DataTreeObjectParameter("description")] string description = "")
        {
            this.LockStateId = lockStateId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte LockStateId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => LockStateId;

        public const int PDL_MIN = 1;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"RDMLockStateDescription: {LockStateId} - {Description}";
        }

        public static RDMLockStateDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.LOCK_STATE_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMLockStateDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMLockStateDescription(
                lockStateId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.LockStateId));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
