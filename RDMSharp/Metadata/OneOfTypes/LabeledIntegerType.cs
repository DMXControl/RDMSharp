using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct LabeledIntegerType
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("value")]
        public readonly long Value { get; }

        [JsonConstructor]
        public LabeledIntegerType(
            string name,
            long value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value} -> {Name}";
        }
    }
}
