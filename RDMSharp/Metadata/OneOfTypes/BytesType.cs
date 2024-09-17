using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class BytesType : CommonPropertiesForNamed
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

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public string Type { get; }
        [JsonPropertyName("format")]
        [JsonPropertyOrder(11)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Format { get; }
        [JsonPropertyName("minLength")]
        [JsonPropertyOrder(12)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MinLength { get; }
        [JsonPropertyName("maxLength")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MaxLength { get; }


        [JsonConstructor]
        public BytesType(string name,
                         string? displayName,
                         string? notes,
                         string[]? resources,
                         string type,
                         string? format,
                         ulong? minLength,
                         ulong? maxLength) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Format = format;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
