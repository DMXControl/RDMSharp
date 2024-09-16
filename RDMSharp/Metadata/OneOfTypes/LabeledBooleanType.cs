using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct LabeledBooleanType
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("value")]
        public readonly bool Value { get; }

        [JsonConstructor]
        public LabeledBooleanType(
            string name,
            bool value)
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
