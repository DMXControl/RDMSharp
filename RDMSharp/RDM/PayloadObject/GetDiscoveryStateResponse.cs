using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.DISCOVERY_STATE, Command.ECommandDublicate.GetResponse)]
public class GetDiscoveryStateResponse : AbstractRDMPayloadObject
{
    public GetDiscoveryStateResponse(
        ushort endpointId = default,
        ushort deviceCount = default,
        ERDM_DiscoveryState discoveryState = default)
    {
        this.EndpointId = endpointId;
        this.DeviceCount = deviceCount;
        this.DiscoveryState = discoveryState;
    }

    [DataTreeObjectConstructor]
    public GetDiscoveryStateResponse(
        [DataTreeObjectParameter("endpoint_id")] ushort endpointId,
        [DataTreeObjectParameter("device_count")] ushort deviceCount,
        [DataTreeObjectParameter("state")] byte discoveryState) :
        this(endpointId, deviceCount, (ERDM_DiscoveryState)discoveryState)
    {
    }

    [DataTreeObjectProperty("endpoint_id", 0)]
    public ushort EndpointId { get; private set; }
    [DataTreeObjectProperty("device_count", 1)]
    public ushort DeviceCount { get; private set; }
    [DataTreeObjectProperty("state", 2)]
    public ERDM_DiscoveryState DiscoveryState { get; private set; }
    public const int PDL = 0x05;

    public override string ToString()
    {
        return $"Endpoint: {EndpointId} - DiscoveryState: {DiscoveryState} DeviceCount: {DeviceCount}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.EndpointId));
        data.AddRange(Tools.ValueToData(this.DeviceCount));
        data.AddRange(Tools.ValueToData(this.DiscoveryState));
        return data.ToArray();
    }
}