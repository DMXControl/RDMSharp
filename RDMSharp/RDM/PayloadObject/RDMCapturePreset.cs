using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.CAPTURE_PRESET, Command.ECommandDublicate.SetRequest)]
public class RDMCapturePreset : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMCapturePreset(
        [DataTreeObjectParameter("scene_num")] ushort sceneId,
        [DataTreeObjectParameter("up_fade_time")] double upFadeTime,
        [DataTreeObjectParameter("down_fade_time")] double downFadeTime,
        [DataTreeObjectParameter("wait_time")] double waitTime)
    {
        this.SceneId = sceneId;
        this.UpFadeTime = upFadeTime;
        this.DownFadeTime = downFadeTime;
        this.WaitTime = waitTime;
    }

    [DataTreeObjectProperty("scene_num", 0)]
    public ushort SceneId { get; private set; }
    [DataTreeObjectProperty("up_fade_time", 1)]
    public double UpFadeTime { get; private set; }
    [DataTreeObjectProperty("down_fade_time", 2)]
    public double DownFadeTime { get; private set; }
    [DataTreeObjectProperty("wait_time", 3)]
    public double WaitTime { get; private set; }

    public const int PDL_small = 0x02;
    public const int PDL_big = 0x08;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMPresetStatus");
        b.AppendLine($"SceneId:      {SceneId}");
        b.AppendLine($"UpFadeTime:   {UpFadeTime}s");
        b.AppendLine($"DownFadeTime: {DownFadeTime}s");
        b.AppendLine($"WaitTime:     {WaitTime}s");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.SceneId));
        data.AddRange(Tools.ValueToData((ushort)(this.UpFadeTime * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.DownFadeTime * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.WaitTime * 10)));
        return data.ToArray();
    }
}