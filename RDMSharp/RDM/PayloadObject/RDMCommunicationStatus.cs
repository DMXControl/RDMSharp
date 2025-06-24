using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.COMMS_STATUS, Command.ECommandDublicate.GetResponse)]
    public class RDMCommunicationStatus : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMCommunicationStatus(
            [DataTreeObjectParameter("short_message")] ushort shortMessage = 0,
            [DataTreeObjectParameter("length_mismatch")] ushort lengthMismatch = 0,
            [DataTreeObjectParameter("checksum_fail")] ushort checksumFail = 0)
        {
            this.ShortMessage = shortMessage;
            this.LengthMismatch = lengthMismatch;
            this.ChecksumFail = checksumFail;
        }

        [DataTreeObjectProperty("scopeString", 0)]
        public ushort ShortMessage { get; private set; }
        [DataTreeObjectProperty("length_mismatch", 1)]
        public ushort LengthMismatch { get; private set; }
        [DataTreeObjectProperty("checksum_fail", 2)]
        public ushort ChecksumFail { get; private set; }

        public const int PDL = 6;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMProxiedDeviceCount");
            b.AppendLine($"ShortMessage:   {ShortMessage}");
            b.AppendLine($"LengthMismatch: {LengthMismatch}");
            b.AppendLine($"ChecksumFail:   {ChecksumFail}");

            return b.ToString();
        }

        public static RDMCommunicationStatus FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.COMMS_STATUS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMCommunicationStatus FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new RDMCommunicationStatus(
                shortMessage: Tools.DataToUShort(ref data),
                lengthMismatch: Tools.DataToUShort(ref data),
                checksumFail: Tools.DataToUShort(ref data)
                );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ShortMessage));
            data.AddRange(Tools.ValueToData(this.LengthMismatch));
            data.AddRange(Tools.ValueToData(this.ChecksumFail));
            return data.ToArray();
        }
    }
}