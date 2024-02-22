using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetIdentifyEndpoint : AbstractRDMPayloadObject
    {
        public GetSetIdentifyEndpoint(
            ushort endpointId = default,
            bool identifyState = default)
        {
            this.EndpointId = endpointId;
            this.IdentifyState = identifyState;
        }

        public ushort EndpointId { get; private set; }
        public bool IdentifyState { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - Identify: {IdentifyState}";
        }

        public static GetSetIdentifyEndpoint FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.IDENTIFY_ENDPOINT) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetIdentifyEndpoint FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetIdentifyEndpoint(
                endpointId: Tools.DataToUShort(ref data),
                identifyState: Tools.DataToBool(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.IdentifyState));
            return data.ToArray();
        }
    }
}