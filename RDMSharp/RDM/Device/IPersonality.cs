using System.Collections.Generic;

namespace RDMSharp;

public interface IPersonality
{
    byte ID { get; }
    ushort SlotCount { get; }
    string Description { get; }
    IReadOnlyDictionary<ushort, Slot> Slots { get; }
}
