using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct Range<T>
    {
        [JsonPropertyName("minimum")]
        public readonly T Minimum { get; }

        [JsonPropertyName("maximum")]
        public readonly T Maximum { get; }

        [JsonConstructor]
        public Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public override string ToString()
        {
            return $"Range: {Minimum:X4} - {Maximum:X4}";
        }
    }
}
