using System.Collections.Generic;

namespace RDMSharp
{
    public class RDMOutputResponseTimeDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        public RDMOutputResponseTimeDescription(
            byte outputResponseTimeId = 1,
            string description = "")
        {
            this.OutputResponseTimeId = outputResponseTimeId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte OutputResponseTimeId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => OutputResponseTimeId;

        public const int PDL_MIN = 1;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"RDMOutputResponseTimeDescription: {OutputResponseTimeId} - {Description}";
        }

        public static RDMOutputResponseTimeDescription FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMOutputResponseTimeDescription FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new RDMOutputResponseTimeDescription(
                outputResponseTimeId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.OutputResponseTimeId));
            data.AddRange(Tools.ValueToData(this.Description, 32));
            return data.ToArray();
        }
    }
}
