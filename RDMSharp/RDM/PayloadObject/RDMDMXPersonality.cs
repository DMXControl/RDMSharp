using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMDMXPersonality : AbstractRDMPayloadObjectOneOf
    {
        public RDMDMXPersonality(
            byte currentPersonality = 0,
            byte ofPersonalities = 0)
        {
            this.CurrentPersonality = currentPersonality;
            this.OfPersonalities = ofPersonalities;
        }

        public byte CurrentPersonality { get; private set; }
        public byte OfPersonalities { get; private set; }

        public override Type IndexType => CurrentPersonality.GetType();
        public override object MinIndex => (byte)1;
        public override object Index => CurrentPersonality;
        public override object Count => OfPersonalities;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"{CurrentPersonality} of {OfPersonalities}";
        }
        public static RDMDMXPersonality FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DMX_PERSONALITY) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMXPersonality FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new RDMDMXPersonality(
                currentPersonality: Tools.DataToByte(ref data),
                ofPersonalities: Tools.DataToByte(ref data)
                );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.CurrentPersonality));
            data.AddRange(Tools.ValueToData(this.OfPersonalities));
            return data.ToArray();
        }
    }
}
