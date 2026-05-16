using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.MINIMUM_LEVEL, Command.ECommandDublicate.GetResponse)]
[DataTreeObject(ERDM_Parameter.MINIMUM_LEVEL, Command.ECommandDublicate.SetRequest)]
public class RDMMinimumLevel : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMMinimumLevel(
        [DataTreeObjectParameter("min_level_increasing")] ushort minimumLevelIncrease = 0,
        [DataTreeObjectParameter("min_level_decreasing")] ushort minimumLevelDecrease = 0,
        [DataTreeObjectParameter("on_below_min")] bool onBelowMinimum = false)
    {
        this.MinimumLevelIncrease = minimumLevelIncrease;
        this.MinimumLevelDecrease = minimumLevelDecrease;
        this.OnBelowMinimum = onBelowMinimum;
    }

    [DataTreeObjectProperty("min_level_increasing", 0)]
    public ushort MinimumLevelIncrease { get; private set; }
    [DataTreeObjectProperty("min_level_decreasing", 1)]
    public ushort MinimumLevelDecrease { get; private set; }
    [DataTreeObjectProperty("on_below_min", 2)]
    public bool OnBelowMinimum { get; private set; }
    public const int PDL = 5;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMMinimumLevel");
        b.AppendLine($"MinimumLevelIncrease: {MinimumLevelIncrease}");
        b.AppendLine($"MinimumLevelDecrease: {MinimumLevelDecrease}");
        b.AppendLine($"OnBelowMinimum:       {OnBelowMinimum}");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.MinimumLevelIncrease));
        data.AddRange(Tools.ValueToData(this.MinimumLevelDecrease));
        data.AddRange(Tools.ValueToData(this.OnBelowMinimum));
        return data.ToArray();
    }
}