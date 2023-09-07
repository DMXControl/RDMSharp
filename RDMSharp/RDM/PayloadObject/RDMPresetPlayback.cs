using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMPresetPlayback : AbstractRDMPayloadObject
    {
        public RDMPresetPlayback(
            ushort mode = 0,
            byte level = 0)
        {
            this.Mode = mode;
            this.Level = level;
        }

        public ERDM_PresetPlayback EMode { get { return (ERDM_PresetPlayback)this.Mode; } }
        public ushort Mode { get; private set; }
        public byte Level { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMProxiedDeviceCount");
            b.AppendLine($"Mode:  {EMode}({Mode})");
            b.AppendLine($"Level: {Level}");

            return b.ToString();
        }

        public static RDMPresetPlayback FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.PRESET_PLAYBACK) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPresetPlayback FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMPresetPlayback(
                mode: Tools.DataToUShort(ref data),
                level: Tools.DataToByte(ref data)
                );

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.Mode));
            data.AddRange(Tools.ValueToData(this.Level));
            return data.ToArray();
        }
    }
}