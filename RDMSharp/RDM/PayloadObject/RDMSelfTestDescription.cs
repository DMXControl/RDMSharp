using System;
using System.Collections.Generic;
using System.Text;

namespace RDMSharp
{
    public class RDMSelfTestDescription : AbstractRDMPayloadObject
    {
        public RDMSelfTestDescription(
            byte selfTestRequester = 0,
            string description = "")
        {
            this.SelfTestRequester = selfTestRequester;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        //ToDo figure out with Hardware if is bit or byte! pgrote 16.11.2021
        public byte SelfTestRequester { get; private set; }
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.SELF_TEST_DESCRIPTION) return null;
            if (msg.PDL < PDL_MIN) throw new Exception($"PDL {msg.PDL} < {PDL_MIN}");
            if (msg.PDL > PDL_MAX) throw new Exception($"PDL {msg.PDL} > {PDL_MAX}");

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMSelfTestDescription FromPayloadData(byte[] data)
        {
            if (data.Length < PDL_MIN) throw new Exception($"PDL {data.Length} < {PDL_MIN}");
            if (data.Length > PDL_MAX) throw new Exception($"PDL {data.Length} > {PDL_MAX}");

            var i = new RDMSelfTestDescription(
                selfTestRequester: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

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