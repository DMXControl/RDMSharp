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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_MODE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointMode FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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