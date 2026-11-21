using RDMSharp.Metadata;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

public class EndpointDescriptor : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public EndpointDescriptor(
        [DataTreeObjectParameter("id")] ushort endpointId = 0,
        [DataTreeObjectParameter("type")] byte endpointType = 0) : this(endpointId, (ERDM_EndpointType)endpointType)
    {
    }

    public EndpointDescriptor(ushort endpointId = 0, ERDM_EndpointType endpointType = 0)
    {
        this.EndpointId = endpointId;
        this.EndpointType = endpointType;
    }


    [DataTreeObjectProperty("id", 0)]
    public ushort EndpointId { get; private set; }

    [DataTreeObjectProperty("type", 1)]
    public ERDM_EndpointType EndpointType { get; private set; }

    public const int PDL = 3;

    public override string ToString()
    {
        return $"Id: {EndpointId} EndpointType: {EndpointType}";
    }
    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.EndpointType));
        return data.ToArray();
    }
}