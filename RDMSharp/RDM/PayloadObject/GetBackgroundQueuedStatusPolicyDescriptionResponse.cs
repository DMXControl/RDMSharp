using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
    public class GetBackgroundQueuedStatusPolicyDescriptionResponse : AbstractRDMPayloadObject, IRDMPayloadObjectIndex, IRDMPayloadObjectOneOfDescription
    {
        [DataTreeObjectConstructor]
        public GetBackgroundQueuedStatusPolicyDescriptionResponse(
            [DataTreeObjectParameter("policy")] byte policyId = 1,
            [DataTreeObjectParameter("description")] string description = default)
        {
            this.PolicyId = policyId;

            if (string.IsNullOrWhiteSpace(description))
                return;

            if (description.Length > 32)
                description = description.Substring(0, 32);

            this.Description = description;
        }

        public ERDM_PolicyType Policy { get { return (ERDM_PolicyType)PolicyId; } }
        public byte PolicyId { get; private set; }
        public string Description { get; private set; }

        public object MinIndex => (byte)1;
        public object Index => PolicyId;

        public const int PDL_MIN = 0x01;
        public const int PDL_MAX = PDL_MIN + 32;

        public override string ToString()
        {
            return $"Policy: {Policy} - Description: {Description}";
        }
        public static GetBackgroundQueuedStatusPolicyDescriptionResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDLRange(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION, PDL_MIN, PDL_MAX);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBackgroundQueuedStatusPolicyDescriptionResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDLRange(data, PDL_MIN, PDL_MAX);

            var i = new GetBackgroundQueuedStatusPolicyDescriptionResponse(
                policyId: Tools.DataToByte(ref data),
                description: Tools.DataToString(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.PolicyId));
            data.AddRange(Tools.ValueToData(this.Description));
            return data.ToArray();
        }
    }
}
