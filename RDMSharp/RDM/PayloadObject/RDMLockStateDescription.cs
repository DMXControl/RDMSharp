using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.LOCK_STATE_DESCRIPTION, Command.ECommandDublicate.GetResponse)]
public class RDMLockStateDescription : AbstractRDMPayloadObject, IRDMPayloadObjectIndex, IRDMPayloadObjectOneOfDescription
{
    [DataTreeObjectConstructor]
    public RDMLockStateDescription(
        [DataTreeObjectParameter("state")] byte lockStateId = default,
        [DataTreeObjectParameter("description")] string description = "")
    {
        this.LockStateId = lockStateId;

        if (string.IsNullOrWhiteSpace(description))
            return;

        if (description.Length > 32)
            description = description.Substring(0, 32);

        this.Description = description;
    }

    [DataTreeObjectProperty("state", 0)]
    public byte LockStateId { get; private set; }
    [DataTreeObjectProperty("description", 1)]
    public string Description { get; private set; }

    public object MinIndex => (byte)1;
    public object Index => LockStateId;

    public const int PDL_MIN = 1;
    public const int PDL_MAX = PDL_MIN + 32;

    public override string ToString()
    {
        return $"RDMLockStateDescription: {LockStateId} - {Description}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.LockStateId));
        data.AddRange(Tools.ValueToData(this.Description, 32));
        return data.ToArray();
    }
}
