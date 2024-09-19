using RDMSharp.Metadata.JSON.Converter;
using RDMSharp.RDM;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
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
    public class IntegerType<T> : CommonPropertiesForNamed
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
        public override string Notes { get; }
        [JsonPropertyName("resources")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string[] Resources { get; }

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public EIntegerType Type { get; }
        [JsonPropertyName("labels")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LabeledIntegerType[] Labels { get; }
        [JsonPropertyName("restrictToLabeled")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RestrictToLabeled { get; }
        [JsonPropertyName("ranges")]
        [JsonPropertyOrder(11)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Range<T>[] Ranges { get; }
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
                           string displayName,
                           string notes,
                           string[] resources,
                           EIntegerType type,
                           LabeledIntegerType[] labels,
                           bool? restrictToLabeled,
                           Range<T>[] ranges,
                           ERDM_SensorUnit? units,
                           int? prefixPower,
                           int? prefixBase) : base()
        {
            T dummy = default;
            switch (dummy)
            {
                case sbyte when type is not EIntegerType.Int8:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.Int8}\"");

                case byte when type is not EIntegerType.UInt8:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.UInt8}\"");

                case short when type is not EIntegerType.Int16:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.Int16}\"");

                case ushort when type is not EIntegerType.UInt16:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.UInt16}\"");

                case int when type is not EIntegerType.Int32:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.Int32}\"");

                case uint when type is not EIntegerType.UInt32:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.UInt32}\"");

                case long when type is not EIntegerType.Int64:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.Int64}\"");

                case ulong when type is not EIntegerType.UInt64:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.UInt64}\"");
#if NET7_0_OR_GREATER
                case Int128 when type is not EIntegerType.Int128:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.Int128}\"");

                case UInt128 when type is not EIntegerType.UInt128:
                    throw new ArgumentException($"Argument {nameof(type)} has to be \"{EIntegerType.UInt128}\"");
#endif
            }

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

        public override PDL GetDataLength()
        {
            switch (Type)
            {
                case EIntegerType.Int8:
                case EIntegerType.UInt8:
                    return new PDL(1);

                case EIntegerType.Int16:
                case EIntegerType.UInt16:
                    return new PDL(2);

                case EIntegerType.Int32:
                case EIntegerType.UInt32:
                    return new PDL(4);

                case EIntegerType.Int64:
                case EIntegerType.UInt64:
                    return new PDL(8);

#if NET7_0_OR_GREATER
                case EIntegerType.Int128:
                case EIntegerType.UInt128:
#endif
                default:
                    return new PDL(16);
            }
        }
    }
}
