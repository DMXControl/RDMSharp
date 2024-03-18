using System.Collections.Generic;

namespace RDMSharp
{
    public class GetDiscoveryStateResponse : AbstractRDMPayloadObject
    {
        public GetDiscoveryStateResponse(
            ushort endpointId = default,
            ushort deviceCount = default,
            ERDM_DiscoveryState discoveryState = default)
        {
            this.EndpointId = endpointId;
            this.DeviceCount = deviceCount;
            this.DiscoveryState = discoveryState;
        }

        public ushort EndpointId { get; private set; }
        public ushort DeviceCount { get; private set; }
        public ERDM_DiscoveryState DiscoveryState { get; private set; }
        public const int PDL = 0x05;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - DiscoveryState: {DiscoveryState} DeviceCount: {DeviceCount}";
        }

        public static GetDiscoveryStateResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DISCOVERY_STATE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetDiscoveryStateResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetDiscoveryStateResponse(
                endpointId: Tools.DataToUShort(ref data),
                deviceCount: Tools.DataToUShort(ref data),
                discoveryState: Tools.DataToEnum<ERDM_DiscoveryState>(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.EndpointId));
            data.AddRange(Tools.ValueToData(this.DeviceCount));
            data.AddRange(Tools.ValueToData(this.DiscoveryState));
            return data.ToArray();
        }
    }
}