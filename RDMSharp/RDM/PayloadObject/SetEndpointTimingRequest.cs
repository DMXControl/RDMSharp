using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.ENDPOINT_TIMING, Command.ECommandDublicate.SetRequest)]

public class SetEndpointTimingRequest : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public SetEndpointTimingRequest(
        [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
        [DataTreeObjectParameter("setting")] byte timingId = default)
    {
        this.EndpointId = endpointId;
        this.TimingId = timingId;
    }

    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("setting", 1)]
    public byte TimingId { get; private set; }
    public const int PDL = 3;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} TimingId: {TimingId}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.TimingId));
        return data.ToArray();
    }
}
