using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DMX_PERSONALITY, Command.ECommandDublicate.GetResponse)]
    public class RDMDMXPersonality : AbstractRDMPayloadObjectOneOf
    {
        [DataTreeObjectConstructor]
        public RDMDMXPersonality(
            [DataTreeObjectParameter("personality")] byte currentPersonality = 1,
            [DataTreeObjectParameter("personality_count")] byte ofPersonalities = 0)
        {
            this.CurrentPersonality = currentPersonality;
            this.OfPersonalities = ofPersonalities;
        }

        [DataTreeObjectProperty("personality", 0)]
        public byte CurrentPersonality { get; private set; }
        [DataTreeObjectDependecieProperty("personality", ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION, Command.ECommandDublicate.GetRequest)]

        [DataTreeObjectProperty("personality_count", 1)]
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_PERSONALITY, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMXPersonality FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
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
