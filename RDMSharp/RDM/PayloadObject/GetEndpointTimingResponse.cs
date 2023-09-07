using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetEndpointTimingResponse : AbstractRDMPayloadObjectOneOf
    {
        public GetEndpointTimingResponse(
            ushort endpointId = default,
            byte timingId = default,
            byte timings = default)
        {
            this.EndpointId = endpointId;
            this.TimingId = timingId;
            this.Timings = timings;
        }

        public ushort EndpointId { get; private set; }
        public byte TimingId { get; private set; }
        public byte Timings { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => TimingId;

        public override object Count => Timings;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION;

        public const int PDL = 4;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} TimingId: {TimingId} of {Timings}";
        }
        public static GetEndpointTimingResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_TIMING) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointTimingResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new GetEndpointTimingResponse(
                endpointId: Tools.DataToUShort(ref data),
                timingId: Tools.DataToByte(ref data),
                timings: Tools.DataToByte(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.TimingId));
            data.AddRange(Tools.ValueToData(this.Timings));
            return data.ToArray();
        }
    }
}
