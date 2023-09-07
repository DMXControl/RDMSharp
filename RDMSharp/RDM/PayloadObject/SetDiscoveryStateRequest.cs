using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class SetDiscoveryStateRequest : AbstractRDMPayloadObject
    {
        public SetDiscoveryStateRequest(
            ushort endpointId = default,
            ERDM_DiscoveryState discoveryState = default)
        {
            this.EndpointId = endpointId;
            this.DiscoveryState = discoveryState;
        }

        public ushort EndpointId { get; private set; }
        public ERDM_DiscoveryState DiscoveryState { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - DiscoveryState: {DiscoveryState}";
        }

        public static SetDiscoveryStateRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DISCOVERY_STATE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static SetDiscoveryStateRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new SetDiscoveryStateRequest(
                endpointId: Tools.DataToUShort(ref data),
                discoveryState: Tools.DataToEnum<ERDM_DiscoveryState>(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.DiscoveryState));
            return data.ToArray();
        }
    }
}