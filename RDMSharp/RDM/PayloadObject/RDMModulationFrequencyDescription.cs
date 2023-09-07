using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDMSharp
{
    public class RDMModulationFrequencyDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMModulationFrequencyDescription(
            byte modulationFrequencyId = 0,
            uint? frequency = null,
            string description = "")
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

        public byte ModulationFrequencyId { get; private set; }
        public uint? Frequency { get; private set; }
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMModulationFrequencyDescription FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new RDMModulationFrequencyDescription(
                modulationFrequencyId: Tools.DataToByte(ref data),
                frequency: Tools.DataToUInt(ref data),
                description: Tools.DataToString(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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

            data.AddRange(Tools.ValueToData(this.Description,32));
            return data.ToArray();
        }
    }
}
