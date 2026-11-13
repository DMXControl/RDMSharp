using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

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

    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("rdm_enabled", 1)]
    public bool RDMTrafficEnabled { get; private set; }
    public const int PDL = 0x03;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} - RDM Traffic Enabled: {RDMTrafficEnabled}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.RDMTrafficEnabled));
        return data.ToArray();
    }
}