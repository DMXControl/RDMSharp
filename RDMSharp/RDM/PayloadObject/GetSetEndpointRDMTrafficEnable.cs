using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.RDM_TRAFFIC_ENABLE, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.RDM_TRAFFIC_ENABLE, Command.ECommandDublicate.SetRequest)]
    public class GetSetEndpointRDMTrafficEnable : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public GetSetEndpointRDMTrafficEnable(
            [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
            [DataTreeObjectParameter("rdm_enabled")] bool rdmTrafficEnabled = default)
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.RDM_TRAFFIC_ENABLE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetSetEndpointRDMTrafficEnable FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new GetSetEndpointRDMTrafficEnable(
                endpointId: Tools.DataToUShort(ref data),
                rdmTrafficEnabled: Tools.DataToBool(ref data));

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