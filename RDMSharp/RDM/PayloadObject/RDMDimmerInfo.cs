using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMDimmerInfo : AbstractRDMPayloadObject
    {
        public RDMDimmerInfo(
            ushort minimumLevelLowerLimit = 0xFFFF,
            ushort minimumLevelUpperLimit = 0xFFFF,
            ushort maximumLevelLowerLimit = 0xFFFF,
            ushort maximumLevelUpperLimit = 0xFFFF,
            byte numberOfSupportedCurves = 0,
            byte levelsResolution = 1,
            bool minimumLevelSplitLevelsSupported = false)
        {
            if (levelsResolution < 0x01 || levelsResolution > 0x10)
                throw new ArgumentOutOfRangeException($"{nameof(levelsResolution)} shold be a value between 1 and 31 but is {levelsResolution}");

            this.MinimumLevelLowerLimit = minimumLevelLowerLimit;
            this.MinimumLevelUpperLimit = minimumLevelUpperLimit;
            this.MaximumLevelLowerLimit = maximumLevelLowerLimit;
            this.MaximumLevelUpperLimit = maximumLevelUpperLimit;
            this.NumberOfSupportedCurves = numberOfSupportedCurves;
            this.LevelsResolution = levelsResolution;
            this.MinimumLevelSplitLevelsSupported = minimumLevelSplitLevelsSupported;
        }

        public ushort MinimumLevelLowerLimit { get; private set; }
        public ushort MinimumLevelUpperLimit { get; private set; }
        public ushort MaximumLevelLowerLimit { get; private set; }
        public ushort MaximumLevelUpperLimit { get; private set; }
        public byte NumberOfSupportedCurves { get; private set; }
        public byte LevelsResolution { get; private set; }
        public bool MinimumLevelSplitLevelsSupported { get; private set; }
        public const int PDL = 11;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMDimmerInfo");
            b.AppendLine($"MinimumLevelLowerLimit:           {MinimumLevelLowerLimit}");
            b.AppendLine($"MinimumLevelUpperLimit:           {MinimumLevelUpperLimit}");
            b.AppendLine($"MaximumLevelLowerLimit:           {MaximumLevelLowerLimit}");
            b.AppendLine($"MaximumLevelUpperLimit:           {MaximumLevelUpperLimit}");
            b.AppendLine($"NumberOfSupportedCurves:          {NumberOfSupportedCurves}");
            b.AppendLine($"LevelsResolution:                 {LevelsResolution}");
            b.AppendLine($"MinimumLevelSplitLevelsSupported: {MinimumLevelSplitLevelsSupported}");

            return b.ToString();
        }

        public static RDMDimmerInfo FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DIMMER_INFO) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDimmerInfo FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMDimmerInfo(
                minimumLevelLowerLimit: Tools.DataToUShort(ref data),
                minimumLevelUpperLimit: Tools.DataToUShort(ref data),
                maximumLevelLowerLimit: Tools.DataToUShort(ref data),
                maximumLevelUpperLimit: Tools.DataToUShort(ref data),
                numberOfSupportedCurves: Tools.DataToByte(ref data),
                levelsResolution: Tools.DataToByte(ref data),
                minimumLevelSplitLevelsSupported: Tools.DataToBool(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.MinimumLevelLowerLimit));
            data.AddRange(Tools.ValueToData(this.MinimumLevelUpperLimit));
            data.AddRange(Tools.ValueToData(this.MaximumLevelLowerLimit));
            data.AddRange(Tools.ValueToData(this.MaximumLevelUpperLimit));
            data.AddRange(Tools.ValueToData(this.NumberOfSupportedCurves));
            data.AddRange(Tools.ValueToData(this.LevelsResolution));
            data.AddRange(Tools.ValueToData(this.MinimumLevelSplitLevelsSupported));
            return data.ToArray();
        }
    }
}