using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class RDMCurveDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMCurveDescription(
            byte curveId = 0,
            string description = "")
        {
            this.CurveId = curveId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte CurveId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => CurveId;

        public const int PDL_MIN = 1;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"RDMCurveDescription: {CurveId} - {Description}";
        }

        public static RDMCurveDescription FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.CURVE_DESCRIPTION) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMCurveDescription FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new RDMCurveDescription(
                curveId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.CurveId));
            data.AddRange(Tools.ValueToData(this.Description,32));
            return data.ToArray();
        }
    }
}
