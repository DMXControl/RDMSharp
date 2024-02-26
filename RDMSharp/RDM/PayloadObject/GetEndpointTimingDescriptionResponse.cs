using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetEndpointTimingDescriptionResponse : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public GetEndpointTimingDescriptionResponse(
            byte timingtId = 1,
            string description = default)
        {
            this.TimingId = timingtId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte TimingId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => TimingId;

        public const int PDL_MIN = 0x01;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"Timing: {TimingId} - Description: {Description}";
        }

        public static GetEndpointTimingDescriptionResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointTimingDescriptionResponse FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new GetEndpointTimingDescriptionResponse(
                timingtId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.TimingId));
            data.AddRange(Tools.ValueToData(this.Description));
            return data.ToArray();
        }
    }
}