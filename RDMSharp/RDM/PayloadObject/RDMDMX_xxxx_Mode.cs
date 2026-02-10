using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.DMX_FAIL_MODE, Command.ECommandDublicate.GetResponse)]
[DataTreeObject(ERDM_Parameter.DMX_FAIL_MODE, Command.ECommandDublicate.SetRequest)]
[DataTreeObject(ERDM_Parameter.DMX_STARTUP_MODE, Command.ECommandDublicate.GetResponse)]
[DataTreeObject(ERDM_Parameter.DMX_STARTUP_MODE, Command.ECommandDublicate.SetRequest)]
public class RDMDMX_xxxx_Mode : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMDMX_xxxx_Mode(
        [DataTreeObjectParameter("scene_num")] ushort scene = 0,
        [DataTreeObjectParameter(ERDM_Parameter.DMX_FAIL_MODE, "loss_of_signal_delay_time"), DataTreeObjectParameter(ERDM_Parameter.DMX_STARTUP_MODE, "startup_delay_time")] double delay = 0,
        [DataTreeObjectParameter("hold_time")] double holdTime = 0,
        [DataTreeObjectParameter("level")] byte level = 0)
    {
        this.Scene = scene;
        this.Delay = delay;
        this.HoldTime = holdTime;
        this.Level = level;
    }

    [DataTreeObjectProperty("scene_num", 0)]
    public ushort Scene { get; private set; }

    [DataTreeObjectProperty(ERDM_Parameter.DMX_FAIL_MODE, "loss_of_signal_delay_time", 1)]
    [DataTreeObjectProperty(ERDM_Parameter.DMX_STARTUP_MODE, "startup_delay_time", 1)]
    public double Delay { get; private set; }

    [DataTreeObjectProperty("hold_time", 2)]
    public double HoldTime { get; private set; }

    [DataTreeObjectProperty("level", 3)]
    public byte Level { get; private set; }
    public const int PDL = 7;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMDMX_xxxx_Mode");
        b.AppendLine($"Scene:    {Scene}");
        b.AppendLine($"Delay:    {Delay}s");
        b.AppendLine($"HoldTime: {HoldTime}s");
        b.AppendLine($"Level:    {Level}");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.Scene));
        data.AddRange(Tools.ValueToData((ushort)(this.Delay * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.HoldTime * 10)));
        data.AddRange(Tools.ValueToData(this.Level));
        return data.ToArray();
    }
}