using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.CURVE, Command.ECommandDublicate.GetResponse)]
public class RDMCurve : AbstractRDMPayloadObjectOneOf
{
    [DataTreeObjectConstructor]
    public RDMCurve(
        [DataTreeObjectParameter("curve")] byte currentCurveId = 1,
        [DataTreeObjectParameter("curve_count")] byte curves = 0)
    {
        this.CurrentCurveId = currentCurveId;
        this.Curves = curves;
    }

    [DataTreeObjectProperty("curve", 0)]
    public byte CurrentCurveId { get; private set; }

    [DataTreeObjectDependecieProperty("curve", ERDM_Parameter.CURVE_DESCRIPTION, Command.ECommandDublicate.GetRequest)]
    [DataTreeObjectProperty("curve_count", 1)]
    public byte Curves { get; private set; }

    public override Type IndexType => typeof(byte);
    public override object MinIndex => (byte)1;

    public override object Index => CurrentCurveId;

    public override object Count => Curves;

    public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.CURVE_DESCRIPTION;

    public const int PDL = 2;

    public override string ToString()
    {
        return $"RDMCurve: {CurrentCurveId} of {Curves}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.CurrentCurveId));
        data.AddRange(Tools.ValueToData(this.Curves));
        return data.ToArray();
    }
}
