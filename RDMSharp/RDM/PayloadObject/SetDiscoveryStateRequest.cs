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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DISCOVERY_STATE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static SetDiscoveryStateRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new SetDiscoveryStateRequest(
                endpointId: Tools.DataToUShort(ref data),
                discoveryState: Tools.DataToEnum<ERDM_DiscoveryState>(ref data));

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