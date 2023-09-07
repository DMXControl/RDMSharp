using System;
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.COMMS_STATUS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMCommunicationStatus FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new RDMCommunicationStatus(
                shortMessage: Tools.DataToUShort(ref data),
                lengthMismatch: Tools.DataToUShort(ref data),
                checksumFail: Tools.DataToUShort(ref data)
                );

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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