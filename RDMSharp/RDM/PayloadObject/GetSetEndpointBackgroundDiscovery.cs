using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BACKGROUND_DISCOVERY, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.BACKGROUND_DISCOVERY, Command.ECommandDublicate.SetRequest)]
    public class GetSetEndpointBackgroundDiscovery : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetEndpointBackgroundDiscovery(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("enabled")] bool backgroundDiscovery = default)
        {
            this.EndpointId = endpointId;
            this.BackgroundDiscovery = backgroundDiscovery;
        }

        [DataTreeObjectProperty("endpoint_id", 0)]
        public ushort EndpointId { get; private set; }
        [DataTreeObjectProperty("enabled", 1)]
        public bool BackgroundDiscovery { get; private set; }
        public const int PDL = 0x03;

        public override string ToString()
        {
            return $"Endpoint: {EndpointId} - BackgroundDiscovery: {BackgroundDiscovery}";
        }

        public static GetSetEndpointBackgroundDiscovery FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.BACKGROUND_DISCOVERY, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointBackgroundDiscovery FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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