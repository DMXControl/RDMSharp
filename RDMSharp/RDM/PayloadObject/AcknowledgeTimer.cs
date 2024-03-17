using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class AcknowledgeTimer : AbstractRDMPayloadObject
    {
        public AcknowledgeTimer(
            TimeSpan estimidatedResponseTime = default) : this((ushort)(estimidatedResponseTime.TotalSeconds / 10))
        {
            if (estimidatedResponseTime.TotalSeconds / 10 > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("The Timer is to long for the Resolution of 16-bit ushort");
        }
        private AcknowledgeTimer(
            ushort _estimidatedResponseTimeRaw = default)
        {
            this.estimidatedResponseTimeRaw = _estimidatedResponseTimeRaw;
            this.EstimidatedResponseTime = TimeSpan.FromSeconds(this.estimidatedResponseTimeRaw * 10);
        }

        public TimeSpan EstimidatedResponseTime { get; private set; }
        private ushort estimidatedResponseTimeRaw;
        public const int PDL = 2;

        public override string ToString()
        {
            return $"AcknowledgeTimer: {EstimidatedResponseTime}";
        }

        public static AcknowledgeTimer FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.ResponseType != ERDM_ResponseType.ACK_TIMER) throw new Exception($"ResponseType is not {ERDM_ResponseType.ACK_TIMER}");

            return FromPayloadData(msg.ParameterData);
        }
        public static AcknowledgeTimer FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new AcknowledgeTimer(Tools.DataToUShort(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.estimidatedResponseTimeRaw));
            return data.ToArray();
        }
    }
}