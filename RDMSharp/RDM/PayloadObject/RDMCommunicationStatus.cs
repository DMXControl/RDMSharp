using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMCommunicationStatus : AbstractRDMPayloadObject
    {
        public RDMCommunicationStatus(
            ushort shortMessage = 0,
            ushort lengthMismatch = 0,
            ushort checksumFail = 0)
        {
            this.ShortMessage = shortMessage;
            this.LengthMismatch = lengthMismatch;
            this.ChecksumFail = checksumFail;
        }

        public ushort ShortMessage { get; private set; }
        public ushort LengthMismatch { get; private set; }
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