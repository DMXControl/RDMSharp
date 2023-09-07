using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetEndpointToUniverse : AbstractRDMPayloadObject
    {
        public GetSetEndpointToUniverse(
            ushort endpointId = default,
            ushort universe = default)
        {
            this.EndpointId = endpointId;
            this.Universe = universe;
        }

        public ushort EndpointId { get; private set; }
        public ushort Universe { get; private set; }
        public const int PDL = 0x04;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} to Universe: {Universe}";
        }

        public static GetSetEndpointToUniverse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_TO_UNIVERSE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointToUniverse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetEndpointToUniverse(
                endpointId: Tools.DataToUShort(ref data),
                universe: Tools.DataToUShort(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.Universe));
            return data.ToArray();
        }
    }
}