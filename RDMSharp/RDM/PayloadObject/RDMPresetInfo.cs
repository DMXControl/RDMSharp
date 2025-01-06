using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.PRESET_INFO, Command.ECommandDublicte.GetResponse)]
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
            [DataTreeObjectParameter("preset_min_fade_time")] ushort minimumPresetFadeTimeSupported = 0,
            [DataTreeObjectParameter("preset_max_fade_time")] ushort maximumPresetFadeTimeSupported = 0,
            [DataTreeObjectParameter("preset_min_wait_time")] ushort minimumPresetWaitTimeSupported = 0,
            [DataTreeObjectParameter("preset_max_wait_time")] ushort maximumPresetWaitTimeSupported = 0,
            [DataTreeObjectParameter("dmx_fail_min_delay_time")] ushort? minimumDMX512FailDelayTimeSupported = null,
            [DataTreeObjectParameter("dmx_fail_max_delay_time")] ushort? maximumDMX512FailDelayTimeSupported = null,
            [DataTreeObjectParameter("dmx_fail_min_hold_time")] ushort? minimumDMX512FailDelayHoldSupported = null,
            [DataTreeObjectParameter("dmx_fail_max_hold_time")] ushort? maximumDMX512FailDelayHoldSupported = null,
            [DataTreeObjectParameter("startup_min_delay_time")] ushort? minimumStartupDelayTimeSupported = null,
            [DataTreeObjectParameter("startup_max_delay_time")] ushort? maximumStartupDelayTimeSupported = null,
            [DataTreeObjectParameter("startup_min_hold_time")] ushort? minimumStartupDelayHoldSupported = null,
            [DataTreeObjectParameter("startup_max_hold_time")] ushort? maximumStartupDelayHoldSupported = null)
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

        public bool LevelFieldSupported { get; private set; }
        public bool PresetSequenceSupported { get; private set; }
        public bool SplitTimesSupported { get; private set; }
        public bool DMX512FailInfiniteDelayTimeSupported { get; private set; }
        public bool DMX512FailInfiniteHoldTimeSupported { get; private set; }
        public bool StartupInfiniteHoldTimeSupported { get; private set; }
        public ushort MaximumSceneNumber { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort MinimumPresetFadeTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort MaximumPresetFadeTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort MinimumPresetWaitTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort MaximumPresetWaitTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MinimumDMX512FailDelayTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MaximumDMX512FailDelayTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MinimumDMX512FailDelayHoldSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MaximumDMX512FailDelayHoldSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MinimumStartupDelayTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MaximumStartupDelayTimeSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>
        public ushort? MinimumStartupDelayHoldSupported { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>r
        public ushort? MaximumStartupDelayHoldSupported { get; private set; }

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
            b.AppendLine($"MinimumPresetFadeTimeSupported:       {MinimumPresetFadeTimeSupported / 10.0}s");
            b.AppendLine($"MaximumPresetFadeTimeSupported:       {MaximumPresetFadeTimeSupported / 10.0}s");
            b.AppendLine($"MinimumPresetWaitTimeSupported:       {MinimumPresetWaitTimeSupported / 10.0}s");
            b.AppendLine($"MaximumPresetWaitTimeSupported:       {MaximumPresetWaitTimeSupported / 10.0}s");

            if (this.MinimumDMX512FailDelayTimeSupported.HasValue)
                b.AppendLine($"MinimumDMX512FailDelayTimeSupported:  {MinimumDMX512FailDelayTimeSupported / 10.0}s");
            if (this.MaximumDMX512FailDelayTimeSupported.HasValue)
                b.AppendLine($"MaximumDMX512FailDelayTimeSupported:  {MaximumDMX512FailDelayTimeSupported / 10.0}s");
            if (this.MinimumDMX512FailDelayHoldSupported.HasValue)
                b.AppendLine($"MinimumDMX512FailDelayHoldSupported:  {MinimumDMX512FailDelayHoldSupported / 10.0}s");
            if (this.MaximumDMX512FailDelayHoldSupported.HasValue)
                b.AppendLine($"MaximumDMX512FailDelayHoldSupported:  {MaximumDMX512FailDelayHoldSupported / 10.0}s");
            if (this.MinimumStartupDelayTimeSupported.HasValue)
                b.AppendLine($"MinimumStartupDelayTimeSupported:     {MinimumStartupDelayTimeSupported / 10.0}s");
            if (this.MaximumStartupDelayTimeSupported.HasValue)
                b.AppendLine($"MaximumStartupDelayTimeSupported:     {MaximumStartupDelayTimeSupported / 10.0}s");
            if (this.MinimumStartupDelayHoldSupported.HasValue)
                b.AppendLine($"MinimumStartupDelayHoldSupported:     {MinimumStartupDelayHoldSupported / 10.0}s");
            if (this.MaximumStartupDelayHoldSupported.HasValue)
                b.AppendLine($"MaximumStartupDelayHoldSupported:     {MaximumStartupDelayHoldSupported / 10.0}s");

            return b.ToString();
        }

        public static RDMPresetInfo FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.PRESET_INFO, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPresetInfo FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMPresetInfo(
                levelFieldSupported: Tools.DataToBool(ref data),
                presetSequenceSupported: Tools.DataToBool(ref data),
                splitTimesSupported: Tools.DataToBool(ref data),
                dmx512FailInfiniteDelayTimeSupported: Tools.DataToBool(ref data),
                dmx512FailInfiniteHoldTimeSupported: Tools.DataToBool(ref data),
                startupInfiniteHoldTimeSupported: Tools.DataToBool(ref data),
                maximumSceneNumber: Tools.DataToUShort(ref data),
                minimumPresetFadeTimeSupported: Tools.DataToUShort(ref data),
                maximumPresetFadeTimeSupported: Tools.DataToUShort(ref data),
                minimumPresetWaitTimeSupported: Tools.DataToUShort(ref data),
                maximumPresetWaitTimeSupported: Tools.DataToUShort(ref data),
                minimumDMX512FailDelayTimeSupported: Tools.DataToUShort(ref data),
                maximumDMX512FailDelayTimeSupported: Tools.DataToUShort(ref data),
                minimumDMX512FailDelayHoldSupported: Tools.DataToUShort(ref data),
                maximumDMX512FailDelayHoldSupported: Tools.DataToUShort(ref data),
                minimumStartupDelayTimeSupported: Tools.DataToUShort(ref data),
                maximumStartupDelayTimeSupported: Tools.DataToUShort(ref data),
                minimumStartupDelayHoldSupported: Tools.DataToUShort(ref data),
                maximumStartupDelayHoldSupported: Tools.DataToUShort(ref data));

            return i;
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
            data.AddRange(Tools.ValueToData(this.MinimumPresetFadeTimeSupported));
            data.AddRange(Tools.ValueToData(this.MaximumPresetFadeTimeSupported));
            data.AddRange(Tools.ValueToData(this.MinimumPresetWaitTimeSupported));
            data.AddRange(Tools.ValueToData(this.MaximumPresetWaitTimeSupported));
            data.AddRange(Tools.ValueToData(this.MinimumDMX512FailDelayTimeSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MaximumDMX512FailDelayTimeSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MinimumDMX512FailDelayHoldSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MaximumDMX512FailDelayHoldSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MinimumStartupDelayTimeSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MaximumStartupDelayTimeSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MinimumStartupDelayHoldSupported ?? ushort.MaxValue));
            data.AddRange(Tools.ValueToData(this.MaximumStartupDelayHoldSupported ?? ushort.MaxValue));
            return data.ToArray();
        }
    }
}