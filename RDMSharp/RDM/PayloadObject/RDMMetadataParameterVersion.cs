using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMMetadataParameterVersion : AbstractRDMPayloadObject
    {
        public RDMMetadataParameterVersion(
            ERDM_Parameter parameterId,
            ushort version)
        {
            this.ParameterId = parameterId;
            this.Version = version;
        }

        public ERDM_Parameter ParameterId { get; private set; }
        public ushort Version { get; private set; }

        public object Index => ParameterId;

        public const int PDL = 0x04;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMMetadataParameterVersion");
            b.AppendLine($"ParameterId:    {ParameterId}");
            b.AppendLine($"Version: {Version}");

            return b.ToString();
        }

        public static RDMMetadataParameterVersion FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.METADATA_PARAMETER_VERSION, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMMetadataParameterVersion FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var parameterId = (ERDM_Parameter)Tools.DataToUShort(ref data);
            var version = Tools.DataToUShort(ref data);

            var i = new RDMMetadataParameterVersion(
                parameterId: parameterId,
                version: version
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ParameterId));
            data.AddRange(Tools.ValueToData(this.Version));
            return data.ToArray();
        }
    }
}
