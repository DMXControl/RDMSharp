using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
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

        public bool IsInRange(T value)
        {
            switch (value)
            {
                case byte v when Minimum is byte min && Maximum is byte max:
                    return min <= v && v <= max;
                case sbyte v when Minimum is sbyte min && Maximum is sbyte max:
                    return min <= v && v <= max;
                case short v when Minimum is short min && Maximum is short max:
                    return min <= v && v <= max;
                case ushort v when Minimum is ushort min && Maximum is ushort max:
                    return min <= v && v <= max;
                case int v when Minimum is int min && Maximum is int max:
                    return min <= v && v <= max;
                case uint v when Minimum is uint min && Maximum is uint max:
                    return min <= v && v <= max;
                case long v when Minimum is long min && Maximum is long max:
                    return min <= v && v <= max;
                case ulong v when Minimum is ulong min && Maximum is ulong max:
                    return min <= v && v <= max;
# if NET7_0_OR_GREATER
                case Int128 v when Minimum is Int128 min && Maximum is Int128 max:
                    return min <= v && v <= max;
                case UInt128 v when Minimum is UInt128 min && Maximum is UInt128 max:
                    return min <= v && v <= max;
#endif
            }
            return false;
        }

        public override string ToString()
        {
            return $"Range: {Minimum:X4} - {Maximum:X4}";
        }
    }
}
