using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp.PayloadObject;

[DataTreeObject(ERDM_Parameter.LOCK_STATE, Command.ECommandDublicate.GetResponse)]
public class GetLockStateResponse : AbstractRDMPayloadObjectOneOf
{
    [DataTreeObjectConstructor]
    public GetLockStateResponse(
        [DataTreeObjectParameter("state")] byte currentLockStateId = 1,
        [DataTreeObjectParameter("state_count")] byte lockStates = 0)
    {
        this.CurrentLockStateId = currentLockStateId;
        this.LockStates = lockStates;
    }

    [DataTreeObjectProperty("state", 0)]
    public byte CurrentLockStateId { get; private set; }

    [DataTreeObjectDependecieProperty("state", ERDM_Parameter.LOCK_STATE_DESCRIPTION, Command.ECommandDublicate.GetRequest)]
    [DataTreeObjectProperty("state_count", 1)]
    public byte LockStates { get; private set; }

    public override Type IndexType => typeof(byte);
    public override object MinIndex => (byte)0;

    public override object Index => CurrentLockStateId;

    public override object Count => LockStates;

    public override ERDM_Parameter DescriptorParameter => ERDM_Parameter.LOCK_STATE_DESCRIPTION;

    public const int PDL = 2;

    public override string ToString()
    {
        return $"RDMLockState: {CurrentLockStateId} of {LockStates}";
    }

    public override byte[] ToPayloadData()
    {
        List<byte> data = new List<byte>();
        data.AddRange(Tools.ValueToData(this.CurrentLockStateId));
        data.AddRange(Tools.ValueToData(this.LockStates));
        return data.ToArray();
    }
}
