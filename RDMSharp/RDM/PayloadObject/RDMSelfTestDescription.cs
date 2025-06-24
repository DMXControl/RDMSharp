using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.SELF_TEST_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class RDMSelfTestDescription : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public RDMSelfTestDescription(
            [DataTreeObjectParameter("self_test_num")] byte selfTestRequester = 0,
            [DataTreeObjectParameter("label")] string description = "")
        {
            this.SelfTestRequester = selfTestRequester;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        [DataTreeObjectProperty("self_test_num", 0)]
        public byte SelfTestRequester { get; private set; }
        [DataTreeObjectProperty("label", 1)]
        public string Description { get; private set; }

        public const int PDL_MIN = 1;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("RDMSelfTestDescription");
            b.AppendLine($"SelfTestRequester: {SelfTestRequester}");
            b.AppendLine($"Description:       {Description}");

            return b.ToString();
        }

        public static RDMSelfTestDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.SELF_TEST_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSelfTestDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMSelfTestDescription(
                selfTestRequester: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.SelfTestRequester));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}