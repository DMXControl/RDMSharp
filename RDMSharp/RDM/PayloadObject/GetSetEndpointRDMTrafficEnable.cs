using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetSetEndpointRDMTrafficEnable : AbstractRDMPayloadObject
    {
        public GetSetEndpointRDMTrafficEnable(
            ushort endpointId = default,
            bool rdmTrafficEnabled = default)
        {
            this.EndpointId = endpointId;
            this.RDMTrafficEnabled = rdmTrafficEnabled;
        }

        public ushort EndpointId { get; private set; }
        public bool RDMTrafficEnabled { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - RDM Traffic Enabled: {RDMTrafficEnabled}";
        }

        public static GetSetEndpointRDMTrafficEnable FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.RDM_TRAFFIC_ENABLE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointRDMTrafficEnable FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new GetSetEndpointRDMTrafficEnable(
                endpointId: Tools.DataToUShort(ref data),
                rdmTrafficEnabled: Tools.DataToBool(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.RDMTrafficEnabled));
            return data.ToArray();
        }
    }
}