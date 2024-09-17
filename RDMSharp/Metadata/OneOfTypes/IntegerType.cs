using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public enum EIntegerType
    {
        [JsonPropertyName("int8")]
        Int8,
        [JsonPropertyName("int16")]
        Int16,
        [JsonPropertyName("int32")]
        Int32,
        [JsonPropertyName("int64")]
        Int64,
        [JsonPropertyName("int128")]
        Int128,
        [JsonPropertyName("uint8")]
        UInt8,
        [JsonPropertyName("uint16")]
        UInt16,
        [JsonPropertyName("uint32")]
        UInt32,
        [JsonPropertyName("uint64")]
        UInt64,
        [JsonPropertyName("uint128")]
        UInt128
    }
    public readonly struct IntegerType<T>
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly EIntegerType Type { get; }
        [JsonPropertyName("labels")]
        public readonly LabeledIntegerType[]? Labels { get; }
        [JsonPropertyName("restrictToLabeled")]
        public readonly bool? RestrictToLabeled { get; }
        [JsonPropertyName("ranges")]
        public readonly Range<T>[]? Ranges { get; }
        [JsonPropertyName("units")]
        public readonly ERDM_SensorUnit? Units { get; }
        [JsonPropertyName("prefixPower")]
        public readonly int? PrefixPower { get; } = 0;
        [JsonPropertyName("prefixBase")]
        public readonly int? PrefixBase { get; } = 10;

        [JsonConstructor]
        public IntegerType(
            string name,
            EIntegerType type,
            LabeledIntegerType[]? labels,
            bool? restrictToLabeled,
            Range<T>[]? ranges,
            ERDM_SensorUnit? units,
            int? prefixPower,
            int? prefixBase)
        {
            Name = name;
            Type = type;
            Labels = labels;
            RestrictToLabeled = restrictToLabeled;
            Ranges = ranges;
            Units = units;
            PrefixPower = prefixPower;
            PrefixBase = prefixBase;
        }

        public override string ToString()
        {
            if (Labels != null)
                return $"{Name} {Type} -> [ {string.Join("; ", Labels.Select(l => l.ToString()))} ]";

            return $"{Name} {Type}";
        }
    }

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
