using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class RDMModulationFrequencyDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex, IRDMPayloadObjectOneOfDescription
    {
        [DataTreeObjectConstructor]
        public RDMModulationFrequencyDescription(
            [DataTreeObjectParameter("setting")] byte modulationFrequencyId = 1,
            [DataTreeObjectParameter("frequency")] uint? frequency = null,
            [DataTreeObjectParameter("description")] string description = "")
        {
            this.ModulationFrequencyId = modulationFrequencyId;

            if (frequency != uint.MaxValue && frequency.HasValue)
                this.Frequency = frequency;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        [DataTreeObjectProperty("setting", 0)]
        public byte ModulationFrequencyId { get; private set; }

        [DataTreeObjectProperty("frequency", 1)]
        public uint? Frequency { get; private set; }

        [DataTreeObjectProperty("description", 2)]
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => ModulationFrequencyId;

        public const int PDL_MIN = 5;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            if (this.Frequency.HasValue)
                return $"RDMModulationFrequencyDescription: {ModulationFrequencyId} ({Frequency}Hz) - {Description}";

            return $"RDMModulationFrequencyDescription: {ModulationFrequencyId} - {Description}";
        }

        public static RDMModulationFrequencyDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMModulationFrequencyDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMModulationFrequencyDescription(
                modulationFrequencyId: Tools.DataToByte(ref data),
                frequency: Tools.DataToUInt(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ModulationFrequencyId));

            if (this.Frequency.HasValue)
                data.AddRange(Tools.ValueToData(this.Frequency));
            else
                data.AddRange(Tools.ValueToData(uint.MaxValue));

            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
