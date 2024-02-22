using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetEndpointBackgroundDiscovery : AbstractRDMPayloadObject
    {
        public GetSetEndpointBackgroundDiscovery(
            ushort endpointId = default,
            bool backgroundDiscovery = default)
        {
            this.EndpointId = endpointId;
            this.BackgroundDiscovery = backgroundDiscovery;
        }

        public ushort EndpointId { get; private set; }
        public bool BackgroundDiscovery { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - BackgroundDiscovery: {BackgroundDiscovery}";
        }

        public static GetSetEndpointBackgroundDiscovery FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.BACKGROUND_DISCOVERY) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointBackgroundDiscovery FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetEndpointBackgroundDiscovery(
                endpointId: Tools.DataToUShort(ref data),
                backgroundDiscovery: Tools.DataToBool(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.BackgroundDiscovery));
            return data.ToArray();
        }
    }
}