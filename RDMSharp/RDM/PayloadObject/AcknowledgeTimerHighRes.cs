using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class AcknowledgeTimerHighRes : AbstractRDMPayloadObject, IAcknowledgeTimer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2208")]
        public AcknowledgeTimerHighRes(
            TimeSpan estimidatedResponseTime = default) : this((ushort)(estimidatedResponseTime.TotalSeconds * 1000.0))
        {
            if (estimidatedResponseTime.TotalSeconds / 10 > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("The Timer is to long for the Resolution of 16-bit ushort");
        }
        private AcknowledgeTimerHighRes(
            ushort _estimidatedResponseTimeRaw = default)
        {
            this.estimidatedResponseTimeRaw = _estimidatedResponseTimeRaw;
            this.EstimidatedResponseTime = TimeSpan.FromSeconds(this.estimidatedResponseTimeRaw / 1000.0);
        }

        public TimeSpan EstimidatedResponseTime { get; private set; }
        private readonly ushort estimidatedResponseTimeRaw;
        public const int PDL = 2;

        public override string ToString()
        {
            return $"AcknowledgeTimerHighRes: {EstimidatedResponseTime}";
        }

        public static AcknowledgeTimerHighRes FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, new ERDM_Parameter[0], PDL);
            if (msg.ResponseType != ERDM_ResponseType.ACK_TIMER_HI_RES) throw new Exception($"ResponseType is not {ERDM_ResponseType.ACK_TIMER_HI_RES}");

            return FromPayloadData(msg.ParameterData);
        }
        public static AcknowledgeTimerHighRes FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new AcknowledgeTimerHighRes(Tools.DataToUShort(ref data));

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