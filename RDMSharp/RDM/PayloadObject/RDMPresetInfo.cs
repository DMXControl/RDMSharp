using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.PRESET_INFO, Command.ECommandDublicate.GetResponse)]
public class RDMPresetInfo : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public RDMPresetInfo(
        [DataTreeObjectParameter("level_field_supported")] bool levelFieldSupported = false,
        [DataTreeObjectParameter("preset_sequence_supported")] bool presetSequenceSupported = false,
        [DataTreeObjectParameter("split_times_supported")] bool splitTimesSupported = false,
        [DataTreeObjectParameter("dmx_fail_infinite_delay_time_supported")] bool dmx512FailInfiniteDelayTimeSupported = false,
        [DataTreeObjectParameter("dmx_fail_infinite_hold_time_supported")] bool dmx512FailInfiniteHoldTimeSupported = false,
        [DataTreeObjectParameter("startup_infinite_hold_time_supported")] bool startupInfiniteHoldTimeSupported = false,
        [DataTreeObjectParameter("max_scene_number")] ushort maximumSceneNumber = 0,
        [DataTreeObjectParameter("preset_min_fade_time")] double minimumPresetFadeTimeSupported = 0,
        [DataTreeObjectParameter("preset_max_fade_time")] double maximumPresetFadeTimeSupported = 0,
        [DataTreeObjectParameter("preset_min_wait_time")] double minimumPresetWaitTimeSupported = 0,
        [DataTreeObjectParameter("preset_max_wait_time")] double maximumPresetWaitTimeSupported = 0,
        [DataTreeObjectParameter("dmx_fail_min_delay_time")] double? minimumDMX512FailDelayTimeSupported = null,
        [DataTreeObjectParameter("dmx_fail_max_delay_time")] double? maximumDMX512FailDelayTimeSupported = null,
        [DataTreeObjectParameter("dmx_fail_min_hold_time")] double? minimumDMX512FailDelayHoldSupported = null,
        [DataTreeObjectParameter("dmx_fail_max_hold_time")] double? maximumDMX512FailDelayHoldSupported = null,
        [DataTreeObjectParameter("startup_min_delay_time")] double? minimumStartupDelayTimeSupported = null,
        [DataTreeObjectParameter("startup_max_delay_time")] double? maximumStartupDelayTimeSupported = null,
        [DataTreeObjectParameter("startup_min_hold_time")] double? minimumStartupDelayHoldSupported = null,
        [DataTreeObjectParameter("startup_max_hold_time")] double? maximumStartupDelayHoldSupported = null)
    {
        this.LevelFieldSupported = levelFieldSupported;
        this.PresetSequenceSupported = presetSequenceSupported;
        this.SplitTimesSupported = splitTimesSupported;
        this.DMX512FailInfiniteDelayTimeSupported = dmx512FailInfiniteDelayTimeSupported;
        this.DMX512FailInfiniteHoldTimeSupported = dmx512FailInfiniteHoldTimeSupported;
        this.StartupInfiniteHoldTimeSupported = startupInfiniteHoldTimeSupported;
        this.MaximumSceneNumber = maximumSceneNumber;
        this.MinimumPresetFadeTimeSupported = minimumPresetFadeTimeSupported;
        this.MaximumPresetFadeTimeSupported = maximumPresetFadeTimeSupported;
        this.MinimumPresetWaitTimeSupported = minimumPresetWaitTimeSupported;
        this.MaximumPresetWaitTimeSupported = maximumPresetWaitTimeSupported;
        this.MinimumDMX512FailDelayTimeSupported = minimumDMX512FailDelayTimeSupported;
        this.MaximumDMX512FailDelayTimeSupported = maximumDMX512FailDelayTimeSupported;
        this.MinimumDMX512FailDelayHoldSupported = minimumDMX512FailDelayHoldSupported;
        this.MaximumDMX512FailDelayHoldSupported = maximumDMX512FailDelayHoldSupported;
        this.MinimumStartupDelayTimeSupported = minimumStartupDelayTimeSupported;
        this.MaximumStartupDelayTimeSupported = maximumStartupDelayTimeSupported;
        this.MinimumStartupDelayHoldSupported = minimumStartupDelayHoldSupported;
        this.MaximumStartupDelayHoldSupported = maximumStartupDelayHoldSupported;
    }

    [DataTreeObjectProperty("level_field_supported", 0)]
    public bool LevelFieldSupported { get; private set; }
    [DataTreeObjectProperty("preset_sequence_supported", 1)]
    public bool PresetSequenceSupported { get; private set; }
    [DataTreeObjectProperty("split_times_supported", 2)]
    public bool SplitTimesSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_infinite_delay_time_supported", 3)]
    public bool DMX512FailInfiniteDelayTimeSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_infinite_hold_time_supported", 4)]
    public bool DMX512FailInfiniteHoldTimeSupported { get; private set; }
    [DataTreeObjectProperty("startup_infinite_hold_time_supported", 5)]
    public bool StartupInfiniteHoldTimeSupported { get; private set; }
    [DataTreeObjectProperty("max_scene_number", 6)]
    public ushort MaximumSceneNumber { get; private set; }

    [DataTreeObjectProperty("preset_min_fade_time", 7)]
    public double MinimumPresetFadeTimeSupported { get; private set; }
    [DataTreeObjectProperty("preset_max_fade_time", 8)]
    public double MaximumPresetFadeTimeSupported { get; private set; }
    [DataTreeObjectProperty("preset_min_wait_time", 9)]
    public double MinimumPresetWaitTimeSupported { get; private set; }
    [DataTreeObjectProperty("preset_max_wait_time", 10)]
    public double MaximumPresetWaitTimeSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_min_delay_time", 11)]
    public double? MinimumDMX512FailDelayTimeSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_max_delay_time", 12)]
    public double? MaximumDMX512FailDelayTimeSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_min_hold_time", 13)]
    public double? MinimumDMX512FailDelayHoldSupported { get; private set; }
    [DataTreeObjectProperty("dmx_fail_max_hold_time", 14)]
    public double? MaximumDMX512FailDelayHoldSupported { get; private set; }
    [DataTreeObjectProperty("startup_min_delay_time", 15)]
    public double? MinimumStartupDelayTimeSupported { get; private set; }
    [DataTreeObjectProperty("startup_max_delay_time", 16)]
    public double? MaximumStartupDelayTimeSupported { get; private set; }
    [DataTreeObjectProperty("startup_min_hold_time", 17)]
    public double? MinimumStartupDelayHoldSupported { get; private set; }
    [DataTreeObjectProperty("startup_max_hold_time", 18)]
    public double? MaximumStartupDelayHoldSupported { get; private set; }

    public const int PDL = 0x20;

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("RDMPresetInfo");
        if (this.LevelFieldSupported)
            b.AppendLine($"LevelFieldSupported:                  {LevelFieldSupported}");
        if (this.PresetSequenceSupported)
            b.AppendLine($"PresetSequenceSupported:              {PresetSequenceSupported}");
        if (this.SplitTimesSupported)
            b.AppendLine($"SplitTimesSupported:                  {SplitTimesSupported}");
        if (this.DMX512FailInfiniteDelayTimeSupported)
            b.AppendLine($"DMX512FailInfiniteDelayTimeSupported: {DMX512FailInfiniteDelayTimeSupported}");
        if (this.DMX512FailInfiniteHoldTimeSupported)
            b.AppendLine($"DMX512FailInfiniteHoldTimeSupported:  {DMX512FailInfiniteHoldTimeSupported}");
        if (this.StartupInfiniteHoldTimeSupported)
            b.AppendLine($"StartupInfiniteHoldTimeSupported:     {StartupInfiniteHoldTimeSupported}");

        b.AppendLine($"MaximumSceneNumber:                   {MaximumSceneNumber}");
        b.AppendLine($"MinimumPresetFadeTimeSupported:       {MinimumPresetFadeTimeSupported}s");
        b.AppendLine($"MaximumPresetFadeTimeSupported:       {MaximumPresetFadeTimeSupported}s");
        b.AppendLine($"MinimumPresetWaitTimeSupported:       {MinimumPresetWaitTimeSupported}s");
        b.AppendLine($"MaximumPresetWaitTimeSupported:       {MaximumPresetWaitTimeSupported}s");

        if (this.MinimumDMX512FailDelayTimeSupported.HasValue)
            b.AppendLine($"MinimumDMX512FailDelayTimeSupported:  {MinimumDMX512FailDelayTimeSupported}s");
        if (this.MaximumDMX512FailDelayTimeSupported.HasValue)
            b.AppendLine($"MaximumDMX512FailDelayTimeSupported:  {MaximumDMX512FailDelayTimeSupported}s");
        if (this.MinimumDMX512FailDelayHoldSupported.HasValue)
            b.AppendLine($"MinimumDMX512FailDelayHoldSupported:  {MinimumDMX512FailDelayHoldSupported}s");
        if (this.MaximumDMX512FailDelayHoldSupported.HasValue)
            b.AppendLine($"MaximumDMX512FailDelayHoldSupported:  {MaximumDMX512FailDelayHoldSupported}s");
        if (this.MinimumStartupDelayTimeSupported.HasValue)
            b.AppendLine($"MinimumStartupDelayTimeSupported:     {MinimumStartupDelayTimeSupported}s");
        if (this.MaximumStartupDelayTimeSupported.HasValue)
            b.AppendLine($"MaximumStartupDelayTimeSupported:     {MaximumStartupDelayTimeSupported}s");
        if (this.MinimumStartupDelayHoldSupported.HasValue)
            b.AppendLine($"MinimumStartupDelayHoldSupported:     {MinimumStartupDelayHoldSupported}s");
        if (this.MaximumStartupDelayHoldSupported.HasValue)
            b.AppendLine($"MaximumStartupDelayHoldSupported:     {MaximumStartupDelayHoldSupported}s");

        return b.ToString();
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.LevelFieldSupported));
        data.AddRange(Tools.ValueToData(this.PresetSequenceSupported));
        data.AddRange(Tools.ValueToData(this.SplitTimesSupported));
        data.AddRange(Tools.ValueToData(this.DMX512FailInfiniteDelayTimeSupported));
        data.AddRange(Tools.ValueToData(this.DMX512FailInfiniteHoldTimeSupported));
        data.AddRange(Tools.ValueToData(this.StartupInfiniteHoldTimeSupported));
        data.AddRange(Tools.ValueToData(this.MaximumSceneNumber));
        data.AddRange(Tools.ValueToData((ushort)(this.MinimumPresetFadeTimeSupported * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.MaximumPresetFadeTimeSupported * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.MinimumPresetWaitTimeSupported * 10)));
        data.AddRange(Tools.ValueToData((ushort)(this.MaximumPresetWaitTimeSupported * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MinimumDMX512FailDelayTimeSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MaximumDMX512FailDelayTimeSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MinimumDMX512FailDelayHoldSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MaximumDMX512FailDelayHoldSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MinimumStartupDelayTimeSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MaximumStartupDelayTimeSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MinimumStartupDelayHoldSupported ?? ushort.MaxValue) * 10)));
        data.AddRange(Tools.ValueToData((ushort)((this.MaximumStartupDelayHoldSupported ?? ushort.MaxValue) * 10)));
        return data.ToArray();
    }
}