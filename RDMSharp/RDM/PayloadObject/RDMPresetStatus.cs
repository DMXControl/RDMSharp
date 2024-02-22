using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMPresetStatus : AbstractRDMPayloadObject
    {
        public RDMPresetStatus(
            ushort sceneId = 0,
            ushort upFadeTime = 0,
            ushort downFadeTime = 0,
            ushort waitTime = 0,
            ERDM_PresetProgrammed programmed = ERDM_PresetProgrammed.NOT_PROGRAMMED)
        {
            this.SceneId = sceneId;
            this.UpFadeTime = upFadeTime;
            this.DownFadeTime = downFadeTime;
            this.WaitTime = waitTime;
            this.Programmed = programmed;
        }

        public ushort SceneId { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>r
        public ushort UpFadeTime { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>r
        public ushort DownFadeTime { get; private set; }
        /// <summary>
        ///  Tenths of a second
        /// </summary>r
        public ushort WaitTime { get; private set; }
        public ERDM_PresetProgrammed Programmed { get; private set; }

        public const int PDL = 0x09;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMPresetStatus");
            b.AppendLine($"SceneId:      {SceneId}");
            b.AppendLine($"UpFadeTime:   {UpFadeTime / 10.0}s");
            b.AppendLine($"DownFadeTime: {DownFadeTime / 10.0}s");
            b.AppendLine($"WaitTime:     {WaitTime / 10.0}s");
            b.AppendLine($"Programmed:   {Programmed}");

            return b.ToString();
        }

        public static RDMPresetStatus FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.PRESET_STATUS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMPresetStatus FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMPresetStatus(
                sceneId: Tools.DataToUShort(ref data),
                upFadeTime: Tools.DataToUShort(ref data),
                downFadeTime: Tools.DataToUShort(ref data),
                waitTime: Tools.DataToUShort(ref data),
                programmed: Tools.DataToEnum<ERDM_PresetProgrammed>(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SceneId));
            data.AddRange(Tools.ValueToData(this.UpFadeTime));
            data.AddRange(Tools.ValueToData(this.DownFadeTime));
            data.AddRange(Tools.ValueToData(this.WaitTime));
            data.AddRange(Tools.ValueToData(this.Programmed));
            return data.ToArray();
        }
    }
}