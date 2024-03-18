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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_TO_UNIVERSE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointToUniverse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetEndpointToUniverse(
                endpointId: Tools.DataToUShort(ref data),
                universe: Tools.DataToUShort(ref data));

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