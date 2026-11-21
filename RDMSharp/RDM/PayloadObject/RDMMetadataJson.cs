using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.METADATA_JSON, Command.ECommandDublicate.GetResponse)]
public class RDMMetadataJson : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMMetadataJson(
        [DataTreeObjectParameter("pid")] ushort parameterId,
        [DataTreeObjectParameter("json")] string json) : this((ERDM_Parameter)parameterId, json)
    {
    }
    public RDMMetadataJson(
        ERDM_Parameter parameterId,
        string json)
    {
        this.ParameterId = parameterId;
        this.JSON = json;
    }

    [DataTreeObjectProperty("pid", 0)]
    public ERDM_Parameter ParameterId { get; private set; }
    [DataTreeObjectProperty("json", 1)]
    public string JSON { get; private set; }

    public object Index => ParameterId;

    public const int PDL_MIN = 0x02;
    public const int PDL_MAX = 0xE7;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMMetadataParameterVersion");
        b.AppendLine($"ParameterId:    {ParameterId}");
        b.AppendLine($"JSON: {JSON}");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.ParameterId));
        data.AddRange(Tools.ValueToData(this.JSON, 0));
        return data.ToArray();
    }
}
