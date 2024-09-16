using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct BooleanType
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("type")]
        public readonly string Type { get; }
        [JsonPropertyName("labels")]
        public readonly LabeledBooleanType[]? Labels { get; }


        [JsonConstructor]
        public BooleanType(
            string name,
            string type,
            LabeledBooleanType[]? labels)
        {
            Name = name;
            Type = type;
            Labels = labels;
        }
        public override string ToString()
        {
            if (Labels == null)
                return Name;

            return $"{Name} [ {string.Join("; ", Labels.Select(l => l.ToString()))} ]";
        }
    }
}