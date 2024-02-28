using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp
{
    public class GeneratedPersonality
    {
        public readonly byte ID;
        public ushort SlotCount => (ushort)slots.Count;
        public readonly string Description;
        private readonly ConcurrentDictionary<ushort, Slot> slots = new ConcurrentDictionary<ushort, Slot>();
        public IReadOnlyDictionary<ushort, Slot> Slots => slots.AsReadOnly();

        public GeneratedPersonality(byte id, string description, params Slot[] _slots)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException($"{0} is not allowed as {id}");
            ID = id;
            Description = description;
            foreach ( var slot in _slots )
            {
                if (!slots.TryAdd(slot.SlotId, slot))
                    throw new Exception($"Cant add Slot: {slot}");
            }
            var maxID = slots.Max(s => s.Key);
            if (slots.Count != maxID + 1)
                throw new ArgumentOutOfRangeException($"The Count not fits the last Slot ID plus one! Count: {slots.Count}, ID:{maxID} ({maxID + 1})");
        }

        public static implicit operator RDMDMXPersonalityDescription(GeneratedPersonality _this)
        {
            return new RDMDMXPersonalityDescription(_this.ID, _this.SlotCount, _this.Description);
        }
        public override string ToString()
        {
            return $"[{ID}] ({SlotCount}) {Description}";
        }
    }
}