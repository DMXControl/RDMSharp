using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class CompoundType
    {
        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("type")]
        public string Type { get; }
        [JsonPropertyName("subtypes")]
        public OneOfTypes[] Subtypes { get; }


        [JsonConstructor]
        public CompoundType(
            string name,
            string type,
            OneOfTypes[] subtypes)
        {
            Name = name;
            Type = type;
            Subtypes = subtypes;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}