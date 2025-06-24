using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.PRESET_PLAYBACK, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.PRESET_PLAYBACK, Command.ECommandDublicate.SetRequest)]
    public class RDMPresetPlayback : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMPresetPlayback(
            [DataTreeObjectParameter("mode")] ushort mode = 0,
            [DataTreeObjectParameter("level")] byte level = 0)
        {
            this.Mode = mode;
            this.Level = level;
        }

        public ERDM_PresetPlayback EMode { get { return (ERDM_PresetPlayback)this.Mode; } }
        [DataTreeObjectProperty("mode", 0)]
        public ushort Mode { get; private set; }
        [DataTreeObjectProperty("level", 1)]
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