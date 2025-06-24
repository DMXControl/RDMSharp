using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DMX_BLOCK_ADDRESS, Command.ECommandDublicate.GetResponse)]
    public class RDMDMXBlockAddress : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMDMXBlockAddress(
            [DataTreeObjectParameter("total_subdevice_footprint")] ushort totalSubDeviceFootprint = 0,
            [DataTreeObjectParameter("base_dmx_address")] ushort baseDMX512Address = 0)
        {
            this.TotalSubDeviceFootprint = totalSubDeviceFootprint;
            this.BaseDMX512Address = baseDMX512Address;
        }

        [DataTreeObjectProperty("total_subdevice_footprint", 0)]
        public ushort TotalSubDeviceFootprint { get; private set; }

        [DataTreeObjectProperty("base_dmx_address", 1)]
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.DMX_BLOCK_ADDRESS, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMDMXBlockAddress FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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