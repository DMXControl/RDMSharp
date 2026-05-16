using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp;

public class GeneratedPersonality : IPersonality
{
    private readonly byte id;
    public byte ID => id;

    public ushort SlotCount => (ushort)slots.Count;

    private readonly string description;
    public string Description => description;

    private readonly ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();
    public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();

    public GeneratedPersonality(byte _id, string _description, params Slot[] _slots)
    {
        if (_id == 0)
            throw new ArgumentOutOfRangeException($"{0} is not allowed as {_id}");
        id = _id;
        description = _description;
        foreach (var slot in _slots)
        {
            if (!slots.TryAdd(slot.SlotId, slot))
                throw new Exception($"Cant add Slot: {slot}");
        }
        var maxID = slots.Max(s => s.Key);
        if (slots.Count != maxID + 1)
            throw new ArgumentOutOfRangeException($"The Count not fits the last Slot ID plus one! Count: {slots.Count}, ID:{maxID} ({maxID + 1})");
    }

    public override string ToString()
    {
        return $"[{ID}] ({SlotCount}) {Description}";
    }
}
