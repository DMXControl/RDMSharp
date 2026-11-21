using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE, Command.ECommandDublicate.GetResponse)]
public class GetEndpointResponderListChangeResponse : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public GetEndpointResponderListChangeResponse(
        [DataTreeObjectParameter("endpoint_id")] ushort endpointId = default,
        [DataTreeObjectParameter("list_change_number")] uint listChangeNumber = default)
    {
        this.EndpointId = endpointId;
        this.ListChangeNumber = listChangeNumber;
    }

    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("list_change_number", 1)]
    public uint ListChangeNumber { get; private set; }
    public const int PDL = 0x06;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} Responder ListChangeNumber: {ListChangeNumber:X}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.ListChangeNumber));
        return data.ToArray();
    }
}