using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct BytesType
    {

        [JsonPropertyName("name")]
        public readonly string Name { get; }

        [JsonPropertyName("type")]
        public readonly string Type { get; }
        [JsonPropertyName("format")]
        public readonly string? Format { get; }
        [JsonPropertyName("minLength")]
        public readonly ulong? MinLength { get; }
        [JsonPropertyName("maxLength")]
        public readonly ulong? MaxLength { get; }


        [JsonConstructor]
        public BytesType(
            string name,
            string type,
            string? format,
            ulong? minLength,
            ulong? maxLength)
        {
            Name = name;
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
