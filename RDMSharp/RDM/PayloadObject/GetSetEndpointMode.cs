using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.ENDPOINT_MODE, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.ENDPOINT_MODE, Command.ECommandDublicate.SetRequest)]
    public class GetSetEndpointMode : AbstractRDMPayloadObject
    {
        public GetSetEndpointMode(
            ushort endpointId = default,
            ERDM_EndpointMode endpointMode = default)
        {
            this.EndpointId = endpointId;
            this.EndpointMode = endpointMode;
        }

        [DataTreeObjectConstructor]
        public GetSetEndpointMode(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("mode")] byte endpointMode = default) :
            this(endpointId, (ERDM_EndpointMode)endpointMode)
        {
        }

        [DataTreeObjectProperty("endpoint_id", 0)]
        public ushort EndpointId { get; private set; }
        [DataTreeObjectProperty("mode", 1)]
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