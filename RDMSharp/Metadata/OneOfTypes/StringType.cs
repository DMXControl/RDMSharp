using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class StringType : CommonPropertiesForNamed
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
        [JsonPropertyOrder(25)]
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
        [JsonPropertyOrder(21)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Format { get; }
        [JsonPropertyName("pattern")]
        [JsonPropertyOrder(22)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Pattern { get; }
        [JsonPropertyName("minLength")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MinLength { get; }
        [JsonPropertyName("maxLength")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MaxLength { get; }
        [JsonPropertyName("minBytes")]
        [JsonPropertyOrder(41)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MinBytes { get; }
        [JsonPropertyName("maxBytes")]
        [JsonPropertyOrder(42)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ulong? MaxBytes { get; }
        [JsonPropertyName("restrictToASCII")]
        [JsonPropertyOrder(51)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RestrictToASCII { get; }


        [JsonConstructor]
        public StringType(string name,
                          string? displayName,
                          string? notes,
                          string[]? resources,
                          string type,
                          string? format,
                          string? pattern,
                          ulong? minLength,
                          ulong? maxLength,
                          ulong? minBytes,
                          ulong? maxBytes,
                          bool? restrictToASCII)
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Format = format;
            Pattern = pattern;
            MinLength = minLength;
            MaxLength = maxLength;
            MinBytes = minBytes;
            MaxBytes = maxBytes;
            RestrictToASCII = restrictToASCII;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
