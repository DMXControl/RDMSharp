using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct BitType
    {
        [JsonConstructor]
        public BitType(string name, string type, ushort index, bool? reserved, bool? valueIfReserved)
        {
            Name = name;
            Type = type;
            Index = index;
            Reserved = reserved;
            ValueIfReserved = valueIfReserved;
        }

        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("type")]
        public readonly string Type { get; }
        [JsonPropertyName("index")]
        public readonly ushort Index { get; }
        [JsonPropertyName("reserved")]
        public readonly bool? Reserved { get; }
        [JsonPropertyName("valueIfReserved")]
        public readonly bool? ValueIfReserved { get; }

        public override string ToString()
        {
            return $"{Index} -> {Name}";
        }
    }
}
