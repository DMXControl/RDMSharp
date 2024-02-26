using System;

namespace RDMSharp
{
    public class GeneratedPersonality
    {
        public readonly byte ID;
        public readonly ushort SlotCount;
        public readonly string Description;

        public GeneratedPersonality(byte id, ushort slotCount, string description)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException($"{0} is not allowed as {id}");
            ID = id;
            SlotCount = slotCount;
            Description = description;
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