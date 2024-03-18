using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMMinimumLevel : AbstractRDMPayloadObject
    {
        public RDMMinimumLevel(
            ushort minimumLevelIncrease = 0,
            ushort minimumLevelDecrease = 0,
            bool onBelowMinimum = false)
        {
            this.MinimumLevelIncrease = minimumLevelIncrease;
            this.MinimumLevelDecrease = minimumLevelDecrease;
            this.OnBelowMinimum = onBelowMinimum;
        }

        public ushort MinimumLevelIncrease { get; private set; }
        public ushort MinimumLevelDecrease { get; private set; }
        public bool OnBelowMinimum { get; private set; }
        public const int PDL = 5;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMMinimumLevel");
            b.AppendLine($"MinimumLevelIncrease: {MinimumLevelIncrease}");
            b.AppendLine($"MinimumLevelDecrease: {MinimumLevelDecrease}");
            b.AppendLine($"OnBelowMinimum:       {OnBelowMinimum}");

            return b.ToString();
        }

        public static RDMMinimumLevel FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.MINIMUM_LEVEL, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMMinimumLevel FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new RDMMinimumLevel(
                minimumLevelIncrease: Tools.DataToUShort(ref data),
                minimumLevelDecrease: Tools.DataToUShort(ref data),
                onBelowMinimum: Tools.DataToBool(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.MinimumLevelIncrease));
            data.AddRange(Tools.ValueToData(this.MinimumLevelDecrease));
            data.AddRange(Tools.ValueToData(this.OnBelowMinimum));
            return data.ToArray();
        }
    }
}