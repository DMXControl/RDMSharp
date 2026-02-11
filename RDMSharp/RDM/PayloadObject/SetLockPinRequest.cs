using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

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

    [DataTreeObjectProperty("new_pin_code", 0)]
    public ushort NewPinCode { get; private set; }
    [DataTreeObjectProperty("current_pin_code", 1)]
    public ushort CurrentPinCode { get; private set; }
    public const int PDL = 4;

    public override string ToString()
    {
        return $"New PIN Code: {NewPinCode} Current PIN Code: {CurrentPinCode}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.NewPinCode));
        data.AddRange(Tools.ValueToData(this.CurrentPinCode));
        return data.ToArray();
    }
}
