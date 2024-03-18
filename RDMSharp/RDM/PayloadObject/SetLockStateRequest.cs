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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.SET_COMMAND, ERDM_Parameter.LOCK_STATE, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static SetLockStateRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);
            var i = new SetLockStateRequest(
                pinCode: Tools.DataToUShort(ref data),
                lockStateId: Tools.DataToByte(ref data));

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
