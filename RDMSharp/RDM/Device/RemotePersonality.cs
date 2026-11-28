using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace RDMSharp;

public class RemotePersonality : IPersonality, INotifyPropertyChanged
{
    private readonly byte id;
    public byte ID => id;

    private readonly ushort slotCount;
    public ushort SlotCount => slotCount;

    private readonly string description;
    public string Description => description;

    private readonly ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<Slot> SlotAdded;

    public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();

    private bool allDataPulled = false;
    public bool AllDataPulled
    {
        get
        {
            return allDataPulled;
        }
        internal set
        {
            if (allDataPulled != value)
                allDataPulled = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllDataPulled)));
        }
    }

    public RemotePersonality(byte _id, string _description, ushort _slotCount)
    {
        this.id = _id;
        this.description = _description;
        this.slotCount = _slotCount;
    }
    internal Slot getOrCreate(ushort slotId)
    {
        if (!slots.TryGetValue(slotId, out Slot slot1))
        {
            slot1 = new Slot(slotId);
            if (slots.TryAdd(slotId, slot1))
                SlotAdded?.InvokeFailSafe(this, slot1);
        }
        return slot1;
    }

    public override string ToString()
    {
        return $"[{ID}] ({SlotCount}) {Description}";
    }
}