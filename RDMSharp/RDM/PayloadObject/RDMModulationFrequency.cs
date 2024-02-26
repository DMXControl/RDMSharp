using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMModulationFrequency : AbstractRDMPayloadObjectOneOf
    {
        public RDMModulationFrequency(
            byte modulationFrequencyId = 1,
            byte modulationFrequencys = 0)
        {
            this.ModulationFrequencyId = modulationFrequencyId;
            this.ModulationFrequencys = modulationFrequencys;
        }

        public byte ModulationFrequencyId { get; private set; }
        public byte ModulationFrequencys { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => ModulationFrequencyId;

        public override object Count => ModulationFrequencys;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"RDMModulationFrequency: {ModulationFrequencyId} of {ModulationFrequencys}";
        }
        public static RDMModulationFrequency FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.MODULATION_FREQUENCY) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMModulationFrequency FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new RDMModulationFrequency(
                modulationFrequencyId: Tools.DataToByte(ref data),
                modulationFrequencys: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ModulationFrequencyId));
            data.AddRange(Tools.ValueToData(this.ModulationFrequencys));
            return data.ToArray();
        }
    }
}
