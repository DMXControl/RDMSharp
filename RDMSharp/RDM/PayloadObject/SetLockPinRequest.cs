
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    public class SetLockPinRequest : AbstractRDMPayloadObject
    {
        public SetLockPinRequest(
            ushort newPinCode = 0,
            ushort currentPinCode = 0)
        {
            this.NewPinCode = newPinCode;
            this.CurrentPinCode = currentPinCode;
        }

        public ushort NewPinCode { get; private set; }
        public ushort CurrentPinCode { get; private set; }
        public const int PDL = 4;

        public override string ToString()
        {
            return $"New PIN Code: {NewPinCode} Current PIN Code: {CurrentPinCode}";
        }
        public static SetLockPinRequest FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (msg.Command != ERDM_Command.SET_COMMAND) return null;
            if (msg.Parameter != ERDM_Parameter.LOCK_PIN) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static SetLockPinRequest FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");
            
            var i = new SetLockPinRequest(
                newPinCode: Tools.DataToUShort(ref data),
                currentPinCode: Tools.DataToUShort(ref data));

            if (data.Length != 0)
                throw new Exception("After deserialization data should be empty!");

            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.NewPinCode));
            data.AddRange(Tools.ValueToData(this.CurrentPinCode));
            return data.ToArray();
        }
    }
}
