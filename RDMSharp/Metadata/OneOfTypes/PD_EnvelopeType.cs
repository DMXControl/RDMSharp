using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class PD_EnvelopeType : CommonPropertiesForNamed
    {
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
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

        [JsonPropertyName("length")]
        [JsonPropertyOrder(3)]
        public byte Length { get; }


        [JsonConstructor]
        public PD_EnvelopeType(string name,
                               string? displayName,
                               string? notes,
                               string[]? resources,
                               byte length) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Length = length;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return $"PDL: {Length} ({Length:X2})";

            return $"PDL: {Length} ({Length:X2}) {base.ToString()}";
        }
    }
}
