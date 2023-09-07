using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetEndpointLabel : AbstractRDMPayloadObject
    {
        public GetSetEndpointLabel(
            ushort endpointId = default,
            string endpointLabel = default)
        {
            this.EndpointId = endpointId;

            if (string.IsNullOrWhiteSpace(endpointLabel))
                return;

            if (endpointLabel.Length > 32)
                endpointLabel = endpointLabel.Substring(0, 32);

            this.EndpointLabel = endpointLabel;
        }

        public ushort EndpointId { get; private set; }
        public string EndpointLabel { get; private set; }
        public const int PDL_MIN = 0x02;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - EndpointLabel: {EndpointLabel}";
        }

        public static GetSetEndpointLabel FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_LABEL) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointLabel FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new GetSetEndpointLabel(
                endpointId: Tools.DataToUShort(ref data),
                endpointLabel: Tools.DataToString(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.EndpointLabel));
            return data.ToArray();
        }
    }
}