using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMPresetInfo : AbstractRDMPayloadObject
    {
        public RDMPresetInfo(
            bool levelFieldSupported = false,
            bool presetSequenceSupported = false,
            bool splitTimesSupported = false,
            bool dmx512FailInfiniteDelayTimeSupported = false,
            bool dmx512FailInfiniteHoldTimeSupported = false,
            bool startupInfiniteHoldTimeSupported = false,
            ushort maximumSceneNumber = 0,
            ushort minimumPresetFadeTimeSupported = 0,
            ushort maximumPresetFadeTimeSupported = 0,
            ushort minimumPresetWaitTimeSupported = 0,
            ushort maximumPresetWaitTimeSupported = 0,
            ushort? minimumDMX512FailDelayTimeSupported = null,
            ushort? maximumDMX512FailDelayTimeSupported = null,
            ushort? minimumDMX512FailDelayHoldSupported = null,
            ushort? maximumDMX512FailDelayHoldSupported = null,
            ushort? minimumStartupDelayTimeSupported = null,
            ushort? maximumStartupDelayTimeSupported = null,
            ushort? minimumStartupDelayHoldSupported = null,
            ushort? maximumStartupDelayHoldSupported = null)
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.PRESET_INFO) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPresetInfo FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

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

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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