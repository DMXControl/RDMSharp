using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
public class GetEndpointTimingDescriptionResponse : AbstractRDMPayloadObject, IRDMPayloadObjectIndex, IRDMPayloadObjectOneOfDescription
{
    [DataTreeObjectConstructor]
    public GetEndpointTimingDescriptionResponse(
        [DataTreeObjectParameter("setting")] byte timingtId = 1,
        [DataTreeObjectParameter("description")] string description = default)
    {
        this.TimingId = timingtId;

        if (string.IsNullOrWhiteSpace(description))
            return;

        if (description.Length > 32)
            description = description.Substring(0, 32);

        this.Description = description;
    }

    [DataTreeObjectProperty("setting", 0)]
    public byte TimingId { get; private set; }
    [DataTreeObjectProperty("description", 1)]
    public string Description { get; private set; }

    public object MinIndex => (byte)1;
    public object Index => TimingId;

    public const int PDL_MIN = 0x01;
    public const int PDL_MAX = PDL_MIN + 32;

    public override string ToString()
    {
        return $"Timing: {TimingId} - Description: {Description}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.TimingId));
        data.AddRange(Tools.ValueToData(this.Description));
        return data.ToArray();
    }
}