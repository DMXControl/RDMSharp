using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMDMXBlockAddress : AbstractRDMPayloadObject
    {
        public RDMDMXBlockAddress(
            ushort totalSubDeviceFootprint = 0,
            ushort baseDMX512Address = 0)
        {
            this.TotalSubDeviceFootprint = totalSubDeviceFootprint;
            this.BaseDMX512Address = baseDMX512Address;
        }

        public ushort TotalSubDeviceFootprint { get; private set; }
        public ushort BaseDMX512Address { get; private set; }
        public const int PDL = 4;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMDMXBlockAddress");
            b.AppendLine($"TotalSubDeviceFootprint: {TotalSubDeviceFootprint}");
            b.AppendLine($"BaseDMX512Address:       {BaseDMX512Address}");

            return b.ToString();
        }

        public static RDMDMXBlockAddress FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.DMX_BLOCK_ADDRESS) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMXBlockAddress FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMDMXBlockAddress(
                totalSubDeviceFootprint: Tools.DataToUShort(ref data),
                baseDMX512Address: Tools.DataToUShort(ref data)
                );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.TotalSubDeviceFootprint));
            data.AddRange(Tools.ValueToData(this.BaseDMX512Address));
            return data.ToArray();
        }
    }
}