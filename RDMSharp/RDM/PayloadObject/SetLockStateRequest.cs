using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class SetLockStateRequest : AbstractRDMPayloadObject
    {
        public SetLockStateRequest(
            ushort pinCode = 0,
            byte lockStateId = 0)
        {
            this.PinCode = pinCode;
            this.LockStateId = lockStateId;
        }

        public ushort PinCode { get; private set; }
        public byte LockStateId { get; private set; }
        public const int PDL = 3;

        public override string ToString()
        {
            return $"PIN Code: {PinCode} Lock State: {LockStateId}";
        }
        public static SetLockStateRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (msg.Command != ERDM_Command.SET_COMMAND) return null;
            if (msg.Parameter != ERDM_Parameter.LOCK_STATE) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static SetLockStateRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            var i = new SetLockStateRequest(
                pinCode: Tools.DataToUShort(ref data),
                lockStateId: Tools.DataToByte(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.PinCode));
            data.AddRange(Tools.ValueToData(this.LockStateId));
            return data.ToArray();
        }
    }
}
