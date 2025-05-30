using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.METADATA_JSON, Command.ECommandDublicate.GetResponse)]
    public class RDMMetadataJson : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMMetadataJson(
            [DataTreeObjectParameter("pid")] ERDM_Parameter parameterId,
            [DataTreeObjectParameter("json")] string json)
        {
            this.ParameterId = parameterId;
            this.JSON = json;
        }

        public ERDM_Parameter ParameterId { get; private set; }
        public string JSON { get; private set; }

        public object Index => ParameterId;

        public const int PDL_MIN = 0x02;
        public const int PDL_MAX = 0xE7;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMMetadataParameterVersion");
            b.AppendLine($"ParameterId:    {ParameterId}");
            b.AppendLine($"JSON: {JSON}");

            return b.ToString();
        }

        public static RDMMetadataJson FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.METADATA_JSON, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMMetadataJson FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var parameterId = (ERDM_Parameter)Tools.DataToUShort(ref data);
            var json = Tools.DataToString(ref data);

            var i = new RDMMetadataJson(
                parameterId: parameterId,
                json: json
            );

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.ParameterId));
            data.AddRange(Tools.ValueToData(this.JSON));
            return data.ToArray();
        }
    }
}
