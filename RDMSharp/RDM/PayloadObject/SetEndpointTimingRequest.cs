using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class SetEndpointTimingRequest : AbstractRDMPayloadObject
    {
        public SetEndpointTimingRequest(
            ushort endpointId = default,
            byte timingId = default)
        {
            this.EndpointId = endpointId;
            this.TimingId = timingId;
        }

        public ushort EndpointId { get; private set; }
        public byte TimingId { get; private set; }
        public byte Timings { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} TimingId: {TimingId}";
        }
        public static SetEndpointTimingRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_TIMING) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static SetEndpointTimingRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new SetEndpointTimingRequest(
                endpointId: Tools.DataToUShort(ref data),
                timingId: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.TimingId));
            return data.ToArray();
        }
    }
}
