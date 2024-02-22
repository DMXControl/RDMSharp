using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetEndpointMode : AbstractRDMPayloadObject
    {
        public GetSetEndpointMode(
            ushort endpointId = default,
            ERDM_EndpointMode endpointMode = default)
        {
            this.EndpointId = endpointId;
            this.EndpointMode = endpointMode;
        }

        public ushort EndpointId { get; private set; }
        public ERDM_EndpointMode EndpointMode { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - EndpointMode: {EndpointMode}";
        }

        public static GetSetEndpointMode FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.ENDPOINT_MODE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointMode FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetEndpointMode(
                endpointId: Tools.DataToUShort(ref data),
                endpointMode: Tools.DataToEnum<ERDM_EndpointMode>(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.EndpointMode));
            return data.ToArray();
        }
    }
}