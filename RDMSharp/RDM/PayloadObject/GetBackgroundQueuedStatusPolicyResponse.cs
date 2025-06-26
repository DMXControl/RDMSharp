using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY, Command.ECommandDublicate.GetResponse)]
    public class GetBackgroundQueuedStatusPolicyResponse : AbstractRDMPayloadObjectOneOf
    {
        [DataTreeObjectConstructor]
        public GetBackgroundQueuedStatusPolicyResponse(
            [DataTreeObjectParameter("policy_setting")] byte policyId = 1,
            [DataTreeObjectParameter("policy_setting_count")] byte policies = default)
        {
            this.PolicyId = policyId;
            this.Policies = policies;
        }

        public ERDM_PolicyType Policy { get { return (ERDM_PolicyType)PolicyId; } }
        [DataTreeObjectProperty("policy_setting", 0)]
        public byte PolicyId { get; private set; }
        [DataTreeObjectProperty("policy_setting_count", 1)]
        public byte Policies { get; private set; }

        public override Type IndexType => typeof(byte);

        public override object MinIndex => (byte)1;
        public override object Index => PolicyId;

        public override object Count => Policies;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"Policy: {Policy} of {Policies}";
        }
        public static GetBackgroundQueuedStatusPolicyResponse FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBackgroundQueuedStatusPolicyResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new GetBackgroundQueuedStatusPolicyResponse(
                policyId: Tools.DataToByte(ref data),
                policies: Tools.DataToByte(ref data));

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.PolicyId));
            data.AddRange(Tools.ValueToData(this.Policies));
            return data.ToArray();
        }
    }
}
