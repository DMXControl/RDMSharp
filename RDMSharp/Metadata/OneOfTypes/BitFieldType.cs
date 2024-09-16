using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct BitFieldType
    {
        [JsonConstructor]
        public BitFieldType(string name, string type, ushort size, bool? valueForUnspecified, BitType[] bits)
        {
            Name = name;
            Type = type;
            Size = size;
            ValueForUnspecified = valueForUnspecified;
            Bits = bits;
        }

        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("type")]
        public readonly string Type { get; }

        [JsonPropertyName("size")]
        public readonly ushort Size { get; }

        [JsonPropertyName("valueForUnspecified")]
        public readonly bool? ValueForUnspecified { get; }
        [JsonPropertyName("bits")]
        public readonly BitType[] Bits { get; }

        public override string ToString()
        {
            return $"{Name} [ {string.Join("; ", Bits.Select(b => b.ToString()))} ]";
        }
    }
}
