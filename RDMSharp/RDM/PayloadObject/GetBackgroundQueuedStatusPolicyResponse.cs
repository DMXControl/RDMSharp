using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetBackgroundQueuedStatusPolicyResponse : AbstractRDMPayloadObjectOneOf
    {
        public GetBackgroundQueuedStatusPolicyResponse(
            byte policyId = 1,
            byte policies = default)
        {
            this.PolicyId = policyId;
            this.Policies = policies;
        }

        public ERDM_PolicyType Policy { get { return (ERDM_PolicyType)PolicyId; } }
        public byte PolicyId { get; private set; }
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
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetBackgroundQueuedStatusPolicyResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
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
