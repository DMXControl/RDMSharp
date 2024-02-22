using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetEndpointResponderListChangeResponse : AbstractRDMPayloadObject
    {
        public GetEndpointResponderListChangeResponse(
            ushort endpointId = default,
            uint listChangeNumber = default)
        {
            this.EndpointId = endpointId;
            this.ListChangeNumber = listChangeNumber;
        }

        public ushort EndpointId { get; private set; }
        public uint ListChangeNumber { get; private set; }
        public const int PDL = 0x06;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} Responder ListChangeNumber: {ListChangeNumber.ToString("X")}";
        }

        public static GetEndpointResponderListChangeResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointResponderListChangeResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetEndpointResponderListChangeResponse(
                endpointId: Tools.DataToUShort(ref data),
                listChangeNumber: Tools.DataToUInt(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.ListChangeNumber));
            return data.ToArray();
        }
    }
}