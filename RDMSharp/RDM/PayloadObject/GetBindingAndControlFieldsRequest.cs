using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.BINDING_CONTROL_FIELDS, Command.ECommandDublicate.GetRequest)]
public class GetBindingAndControlFieldsRequest : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public GetBindingAndControlFieldsRequest(
        [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
        [DataTreeObjectParameter("uid")] UID uid = default)
    {
        this.EndpointId = endpointId;
        this.UID = uid;
    }

    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("uid", 1)]
    public UID UID { get; private set; }
    public const int PDL = 0x08;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} - UID: {UID}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.UID));
        return data.ToArray();
    }
}