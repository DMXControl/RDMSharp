using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class GetLockStateResponse : AbstractRDMPayloadObjectOneOf
    {
        public GetLockStateResponse(
            byte currentLockStateId = 1,
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.LOCK_STATE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static GetLockStateResponse FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new GetLockStateResponse(
                currentLockStateId: Tools.DataToByte(ref data),
                lockStates: Tools.DataToByte(ref data));

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
