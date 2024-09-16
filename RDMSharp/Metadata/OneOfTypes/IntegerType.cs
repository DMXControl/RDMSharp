using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public readonly struct IntegerType
    {
        public enum EType
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
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public readonly EType Type { get; }
        [JsonPropertyName("labels")]
        public readonly LabeledIntegerType[]? Labels { get; }
        [JsonPropertyName("restrictToLabeled")]
        public readonly bool? RestrictToLabeled { get; }
        [JsonPropertyName("ranges")]
        public readonly Range[]? Ranges { get; }
        [JsonPropertyName("units")]
        public readonly ERDM_SensorUnit? Units { get; }
        [JsonPropertyName("prefixPower")]
        public readonly int? PrefixPower { get; } = 0;
        [JsonPropertyName("prefixBase")]
        public readonly int? PrefixBase { get; } = 10;


        [JsonConstructor]
        public IntegerType(
            string name,
            EType type,
            LabeledIntegerType[]? labels,
            bool? restrictToLabeled,
            Range[]? ranges,
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
    public readonly struct Range
    {
#if NET7_0_OR_GREATER
        [JsonPropertyName("minimum")]
        public readonly Int128 Minimum { get; }

        [JsonPropertyName("maximum")]
        public readonly Int128 Maximum { get; }

        [JsonConstructor]
        public Range(Int128 minimum, Int128 maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }
#else
        [JsonPropertyName("minimum")]
        public readonly long Minimum { get; }

        [JsonPropertyName("maximum")]
        public readonly long Maximum { get; }

        [JsonConstructor]
        public Range(long minimum, long maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }
#endif

        public override string ToString()
        {
            return $"Range: {Minimum:X4} - {Maximum:X4}";
        }
    }
}
