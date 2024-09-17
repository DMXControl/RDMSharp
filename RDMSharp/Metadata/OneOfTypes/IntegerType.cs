using Humanizer.Localisation;
using RDMSharp.Metadata.JSON;
using RDMSharp.Metadata.JSON.Converter;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    [JsonConverter(typeof(CustomEnumConverter<EIntegerType>))]
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
    public class  IntegerType<T>: CommonPropertiesForNamed
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
        public EIntegerType Type { get; }
        [JsonPropertyName("labels")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LabeledIntegerType[]? Labels { get; }
        [JsonPropertyName("restrictToLabeled")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RestrictToLabeled { get; }
        [JsonPropertyName("ranges")]
        [JsonPropertyOrder(11)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Range<T>[]? Ranges { get; }
        [JsonPropertyName("units")]
        [JsonPropertyOrder(21)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ERDM_SensorUnit? Units { get; }
        [JsonPropertyName("prefixPower")]
        [JsonPropertyOrder(22)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PrefixPower { get; } = 0;
        [JsonPropertyName("prefixBase")]
        [JsonPropertyOrder(23)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PrefixBase { get; } = 10;

        [JsonConstructor]
        public IntegerType(string name,
                           string? displayName,
                           string? notes,
                           string[]? resources,
                           EIntegerType type,
                           LabeledIntegerType[]? labels,
                           bool? restrictToLabeled,
                           Range<T>[]? ranges,
                           ERDM_SensorUnit? units,
                           int? prefixPower,
                           int? prefixBase) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
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
