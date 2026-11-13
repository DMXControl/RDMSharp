using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.PRESET_STATUS, Command.ECommandDublicate.GetResponse)]
[DataTreeObject(ERDM_Parameter.PRESET_STATUS, Command.ECommandDublicate.SetRequest)]
[DataTreeObject(ERDM_Parameter.CAPTURE_PRESET, Command.ECommandDublicate.SetRequest)]
public class RDMPresetStatus : AbstractRDMPayloadObject
{
    public RDMPresetStatus(
        ushort sceneId = 0,
        double upFadeTime = 0,
        double downFadeTime = 0,
        double waitTime = 0,
        ERDM_PresetProgrammed programmed = ERDM_PresetProgrammed.NOT_PROGRAMMED)
    {
        this.SceneId = sceneId;
        this.UpFadeTime = upFadeTime;
        this.DownFadeTime = downFadeTime;
        this.WaitTime = waitTime;
        this.EProgrammed = programmed;
    }

    [DataTreeObjectConstructor]
    public RDMPresetStatus(
        [DataTreeObjectParameter("scene_num")] ushort sceneId,
        [DataTreeObjectParameter("up_fade_time")] double upFadeTime,
        [DataTreeObjectParameter("down_fade_time")] double downFadeTime,
        [DataTreeObjectParameter("wait_time")] double waitTime,
        [DataTreeObjectParameter("programmed")] byte programmed)
        : this(sceneId, upFadeTime, downFadeTime, waitTime, (ERDM_PresetProgrammed)programmed)
    {
    }

    [DataTreeObjectProperty("scene_num", 0)]
    public ushort SceneId { get; private set; }
    [DataTreeObjectProperty("up_fade_time", 1)]
    public double UpFadeTime { get; private set; }
    [DataTreeObjectProperty("down_fade_time", 2)]
    public double DownFadeTime { get; private set; }
    [DataTreeObjectProperty("wait_time", 3)]
    public double WaitTime { get; private set; }
    public ERDM_PresetProgrammed EProgrammed { get { return (ERDM_PresetProgrammed)Programmed; } private set { Programmed = (byte)value; } }
    [DataTreeObjectProperty("programmed", 4)]
    public byte Programmed { get; private set; }

    public const int PDL = 0x09;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMPresetStatus");
        b.AppendLine($"SceneId:      {SceneId}");
        b.AppendLine($"UpFadeTime:   {UpFadeTime}s");
        b.AppendLine($"DownFadeTime: {DownFadeTime}s");
        b.AppendLine($"WaitTime:     {WaitTime}s");
        b.AppendLine($"Programmed:   {EProgrammed}");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.SceneId));
        data.AddRange(Tools.ValueToData((ushort)(this.UpFadeTime * 1)));
        data.AddRange(Tools.ValueToData((ushort)(this.DownFadeTime * 1)));
        data.AddRange(Tools.ValueToData((ushort)(this.WaitTime * 1)));
        data.AddRange(Tools.ValueToData(this.EProgrammed));
        return data.ToArray();
    }
}