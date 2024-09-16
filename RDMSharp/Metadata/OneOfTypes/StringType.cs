using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct StringType
    {

        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("format")]
        public readonly string? Format { get; }
        [JsonPropertyName("pattern")]
        public readonly string? Pattern { get; }
        [JsonPropertyName("minLength")]
        public readonly ulong? MinLength { get; }
        [JsonPropertyName("maxLength")]
        public readonly ulong? MaxLength { get; }
        [JsonPropertyName("minBytes")]
        public readonly ulong? MinBytes { get; }
        [JsonPropertyName("maxBytes")]
        public readonly ulong? MaxBytes { get; }
        [JsonPropertyName("restrictToASCII")]
        public readonly bool? RestrictToASCII { get; }


        [JsonConstructor]
        public StringType(
            string name,
            string? format,
            string? pattern,
            ulong? minLength,
            ulong? maxLength,
            ulong? minBytes,
            ulong? maxBytes,
            bool? restrictToASCII)
        {
            Name = name;
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
            return Name;
        }
    }
}
