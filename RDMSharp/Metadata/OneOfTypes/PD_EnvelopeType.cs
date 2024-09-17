using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct PD_EnvelopeType
    {

        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("length")]
        public readonly byte Length { get; }


        [JsonConstructor]
        public PD_EnvelopeType(
            string name,
            byte length)
        {
            Name = name;
            Length = length;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return $"PDL: {Length} ({Length:X2})";

            return $"PDL: {Length} ({Length:X2}) {Name}";
        }
    }
}
