using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.LOCK_PIN, Command.ECommandDublicate.SetRequest)]
    public class SetLockPinRequest : AbstractRDMPayloadObject
    {
        [DataTreeObjectConstructor]
        public SetLockPinRequest(
            [DataTreeObjectParameter("new_pin_code")] ushort newPinCode = 0,
            [DataTreeObjectParameter("current_pin_code")] ushort currentPinCode = 0)
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
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.SET_COMMAND, ERDM_Parameter.LOCK_PIN, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static SetLockPinRequest FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

            var i = new SetLockPinRequest(
                newPinCode: Tools.DataToUShort(ref data),
                currentPinCode: Tools.DataToUShort(ref data));

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
