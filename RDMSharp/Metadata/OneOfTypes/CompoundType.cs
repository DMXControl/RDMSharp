using Humanizer.Localisation;
using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class CompoundType : CommonPropertiesForNamed
    {
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string Name { get; }
        [JsonPropertyName("displayName")]
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string DisplayName { get; }
        [JsonPropertyName("notes")]
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string? Notes { get; }
        [JsonPropertyName("resources")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string[]? Resources { get; }

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public string Type { get; }
        [JsonPropertyName("subtypes")]
        [JsonPropertyOrder(11)]
        public OneOfTypes[] Subtypes { get; }


        [JsonConstructor]
        public CompoundType(string name,
                            string? displayName,
                            string? notes,
                            string[]? resources,
                            string type,
                            OneOfTypes[] subtypes)
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Subtypes = subtypes;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}