using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.PRESET_STATUS, Command.ECommandDublicate.SetRequest)]
public class SetPresetStatus : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public SetPresetStatus(
        [DataTreeObjectParameter("scene_num")] ushort sceneId,
        [DataTreeObjectParameter("up_fade_time")] double upFadeTime,
        [DataTreeObjectParameter("down_fade_time")] double downFadeTime,
        [DataTreeObjectParameter("wait_time")] double waitTime,
        [DataTreeObjectParameter("clear_preset")] bool clearPreset)
    {
        this.SceneId = sceneId;
        this.UpFadeTime = upFadeTime;
        this.DownFadeTime = downFadeTime;
        this.WaitTime = waitTime;
        this.Clear = clearPreset;
    }

    [DataTreeObjectProperty("scene_num", 0)]
    public ushort SceneId { get; private set; }
    [DataTreeObjectProperty("up_fade_time", 1)]
    public double UpFadeTime { get; private set; }
    [DataTreeObjectProperty("down_fade_time", 2)]
    public double DownFadeTime { get; private set; }
    [DataTreeObjectProperty("wait_time", 3)]
    public double WaitTime { get; private set; }
    [DataTreeObjectProperty("clear_preset", 4)]
    public bool Clear { get; private set; }

    public const int PDL = 0x09;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("SetPresetStatus");
        b.AppendLine($"SceneId:      {SceneId}");
        b.AppendLine($"UpFadeTime:   {UpFadeTime}s");
        b.AppendLine($"DownFadeTime: {DownFadeTime}s");
        b.AppendLine($"WaitTime:     {WaitTime}s");
        b.AppendLine($"Clear:        {Clear}");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.SceneId));
        data.AddRange(Tools.ValueToData((ushort)(this.UpFadeTime * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.DownFadeTime * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.WaitTime * 10)));
        data.AddRange(Tools.ValueToData(this.Clear));
        return data.ToArray();
    }
}
