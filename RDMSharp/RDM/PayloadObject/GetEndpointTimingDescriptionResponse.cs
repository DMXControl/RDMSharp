using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class GetEndpointTimingDescriptionResponse : AbstractRDMPayloadObject, IRDMPayloadObjectIndex
    {
        [DataTreeObjectConstructor]
        public GetEndpointTimingDescriptionResponse(
            [DataTreeObjectParameter("setting")] byte timingtId = 1,
            [DataTreeObjectParameter("description")] string description = default)
        {
            this.TimingId = timingtId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public byte TimingId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => TimingId;

        public const int PDL_MIN = 0x01;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"Timing: {TimingId} - Description: {Description}";
        }

        public static GetEndpointTimingDescriptionResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetEndpointTimingDescriptionResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new GetEndpointTimingDescriptionResponse(
                timingtId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }

        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.TimingId));
            data.AddRange(Tools.ValueToData(this.Description));
            return data.ToArray();
        }
    }
}