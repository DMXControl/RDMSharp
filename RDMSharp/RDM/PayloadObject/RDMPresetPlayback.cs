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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.PRESET_PLAYBACK, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPresetPlayback FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMPresetPlayback(
                mode: Tools.DataToUShort(ref data),
                level: Tools.DataToByte(ref data)
                );

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