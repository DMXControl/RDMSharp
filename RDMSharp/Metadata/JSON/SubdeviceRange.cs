using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON
{
    public readonly struct SubdeviceRange
    {
        [JsonPropertyName("minimum")]
        public readonly ushort Minimum { get; }

        [JsonPropertyName("maximum")]
        public readonly ushort Maximum { get; }

        [JsonConstructor]
        public SubdeviceRange(ushort minimum, ushort maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }
        public override string ToString()
        {
            return $"Subdevice Range: {Minimum:X4} - {Maximum:X4}";
        }
    }
}
