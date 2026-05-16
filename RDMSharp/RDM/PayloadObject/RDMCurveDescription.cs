using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.CURVE_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
public class RDMCurveDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex, IRDMPayloadObjectOneOfDescription
{
    [DataTreeObjectConstructor]
    public RDMCurveDescription(
        [DataTreeObjectParameter("curve")] byte curveId = 1,
        [DataTreeObjectParameter("description")] string description = "")
    {
        this.CurveId = curveId;

        if (string.IsNullOrWhiteSpace(description))
            return;

        if (description.Length > 32)
            description = description.Substring(0, 32);

        this.Description = description;
    }

    [DataTreeObjectProperty("curve", 0)]
    public byte CurveId { get; private set; }

    [DataTreeObjectProperty("description", 1)]
    public string Description { get; private set; }

    public object MinIndex => (byte)1;
    public object Index => CurveId;

    public const int PDL_MIN = 1;
    public const int PDL_MAX = PDL_MIN + 32;

    public override string ToString()
    {
        return $"RDMCurveDescription: {CurveId} - {Description}";
    }
    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.CurveId));
        data.AddRange(Tools.ValueToData(this.Description, 32));
        return data.ToArray();
    }
}
