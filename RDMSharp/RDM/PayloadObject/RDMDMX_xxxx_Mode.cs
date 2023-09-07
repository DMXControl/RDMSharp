using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMDMX_xxxx_Mode : AbstractRDMPayloadObject
    {
        public RDMDMX_xxxx_Mode(
            ushort scene = 0,
            ushort delay = 0,
            ushort holdTime = 0,
            byte level = 0)
        {
            this.Scene = scene;
            this.Delay = delay;
            this.HoldTime = holdTime;
            this.Level = level;
        }

        public ushort Scene { get; private set; }
        public ushort Delay { get; private set; }
        public ushort HoldTime { get; private set; }
        public byte Level { get; private set; }
        public const int PDL = 7;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMDMXFailMode");
            b.AppendLine($"Scene:    {Scene}");
            b.AppendLine($"Delay:    {Delay}");
            b.AppendLine($"HoldTime: {HoldTime}");
            b.AppendLine($"Level:    {Level}");

            return b.ToString();
        }

        public static RDMDMX_xxxx_Mode FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DMX_FAIL_MODE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMX_xxxx_Mode FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMDMX_xxxx_Mode(
                scene: Tools.DataToUShort(ref data),
                delay: Tools.DataToUShort(ref data),
                holdTime: Tools.DataToUShort(ref data),
                level: Tools.DataToByte(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.Scene));
            data.AddRange(Tools.ValueToData(this.Delay));
            data.AddRange(Tools.ValueToData(this.HoldTime));
            data.AddRange(Tools.ValueToData(this.Level));
            return data.ToArray();
        }
    }
}