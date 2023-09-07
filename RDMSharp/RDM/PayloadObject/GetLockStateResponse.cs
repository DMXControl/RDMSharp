using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetLockStateResponse : AbstractRDMPayloadObjectOneOf
    {
        public GetLockStateResponse(
            byte currentLockStateId = 0,
            byte lockStates = 0)
        {
            this.CurrentLockStateId = currentLockStateId;
            this.LockStates = lockStates;
        }

        public byte CurrentLockStateId { get; private set; }
        public byte LockStates { get; private set; }

        public override Type IndexType => typeof(byte);
        public override object MinIndex => (byte)1;

        public override object Index => CurrentLockStateId;

        public override object Count => LockStates;

        public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.LOCK_STATE_DESCRIPTION;

        public const int PDL = 2;

        public override string ToString()
        {
            return $"RDMLockState: {CurrentLockStateId} of {LockStates}";
        }
        public static GetLockStateResponse FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.LOCK_STATE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static GetLockStateResponse FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new GetLockStateResponse(
                currentLockStateId: Tools.DataToByte(ref data),
                lockStates: Tools.DataToByte(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.CurrentLockStateId));
            data.AddRange(Tools.ValueToData(this.LockStates));
            return data.ToArray();
        }
    }
}
