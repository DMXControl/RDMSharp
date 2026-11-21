using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.LOCK_STATE, Command.ECommandDublicate.SetRequest)]
public class SetLockStateRequest : AbstractRDMPayloadObject
{
    [DataTreeObjectConstructor]
    public SetLockStateRequest(
        [DataTreeObjectParameter("pin_code")] ushort pinCode = 0,
        [DataTreeObjectParameter("state")] byte lockStateId = 0)
    {
        this.PinCode = pinCode;
        this.LockStateId = lockStateId;
    }

    [DataTreeObjectProperty("pin_code", 0)]
    public ushort PinCode { get; private set; }
    [DataTreeObjectProperty("state", 1)]
    public byte LockStateId { get; private set; }
    public const int PDL = 3;

    public override string ToString()
    {
        return $"PIN Code: {PinCode} Lock State: {LockStateId}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.PinCode));
        data.AddRange(Tools.ValueToData(this.LockStateId));
        return data.ToArray();
    }
}
