using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class IntegerType<T> : CommonPropertiesForNamed, IIntegerType
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

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public double PrefixMultiplyer { get; }

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
            validateType<T>(type);
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

            PrefixMultiplyer = Math.Pow(PrefixBase ?? 10, PrefixPower ?? 0);
        }

        private static void validateType<T>(EIntegerType type, T dummy = default)
        {
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
            }
            return new PDL(16);
        }

        private T convertFormatedValueToRaw<T>(object formated)
        {
            if (PrefixMultiplyer == 1)
                return (T)formated;

            object rawValue = null;
            switch (formated)
            {
                case double _double:
                    rawValue = _double / PrefixMultiplyer;
                    break;
                case long _long:
                    rawValue = _long / PrefixMultiplyer;
                    break;
                case ulong _ulong:
                    rawValue = _ulong / PrefixMultiplyer;
                    break;

                default:
                    return (T)formated;
            }

            if (rawValue is not null)
                return (T)Convert.ChangeType(rawValue, typeof(T));

            throw new NotImplementedException();
        }

        private object convertRawValueToFormated(T raw)
        {
            if (PrefixMultiplyer == 1)
                return raw;

            bool isNegativ = Math.Sign(PrefixMultiplyer) == -1;
            bool isDezimal = PrefixPower < 0;

            switch (raw)
            {
                case sbyte int8:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * int8);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * int8);
                    return (ulong)(PrefixMultiplyer * int8);

                case byte uint8:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * uint8);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * uint8);
                    return (ulong)(PrefixMultiplyer * uint8);

                case short int16:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * int16);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * int16);
                    return (ulong)(PrefixMultiplyer * int16);

                case ushort uint16:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * uint16);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * uint16);
                    return (ulong)(PrefixMultiplyer * uint16);

                case int int32:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * int32);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * int32);
                    return (ulong)(PrefixMultiplyer * int32);

                case uint uint32:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * uint32);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * uint32);
                    return (ulong)(PrefixMultiplyer * uint32);

                case long int64:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * int64);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * int64);
                    return (ulong)(PrefixMultiplyer * int64);

                case ulong uint64:
                    if (isDezimal)
                        return (double)(PrefixMultiplyer * uint64);
                    if (isNegativ)
                        return (long)(PrefixMultiplyer * uint64);
                    return (ulong)(PrefixMultiplyer * uint64);

                default:
                    return raw;
            }
            throw new NotImplementedException();
        }

        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            var rawValue = convertFormatedValueToRaw<T>(dataTree.Value);
            if (Ranges != null)
            {
                if (!Ranges.Any(r => r.IsInRange(rawValue)))
                    throw new ArithmeticException("The Value is not in range of any Range");
            }
            var data = Tools.ValueToData(rawValue);

            return data;
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            uint pdl = GetDataLength().Value.Value;

            if (data.Length < pdl)
            {
                issueList.Add(new DataTreeIssue("Given Data not fits PDL"));
                byte[] cloneData = new byte[pdl];
                Array.Copy(data, cloneData, data.Length);
                data = cloneData;
            }

            object value = null;

            switch (this.Type)
            {
                case EIntegerType.Int8:
                    value = Tools.DataToSByte(ref data);
                    break;
                case EIntegerType.UInt8:
                    value = Tools.DataToByte(ref data);
                    break;
                case EIntegerType.Int16:
                    value = Tools.DataToShort(ref data);
                    break;
                case EIntegerType.UInt16:
                    value = Tools.DataToUShort(ref data);
                    break;
                case EIntegerType.Int32:
                    value = Tools.DataToInt(ref data);
                    break;
                case EIntegerType.UInt32:
                    value = Tools.DataToUInt(ref data);
                    break;
                case EIntegerType.Int64:
                    value = Tools.DataToLong(ref data);
                    break;
                case EIntegerType.UInt64:
                    value = Tools.DataToULong(ref data);
                    break;
#if NET7_0_OR_GREATER
                case EIntegerType.Int128:
                    value = Tools.DataToInt128(ref data);
                    break;
                case EIntegerType.UInt128:
                    value = Tools.DataToUInt128(ref data);
                    break;
#endif
            }
            if (Ranges != null)
            {
                if (!Ranges.Any(r => r.IsInRange((T)value)))
                    issueList.Add(new DataTreeIssue("The Value is not in range of any Range"));
            }

            string unit = null;
            if (Units.HasValue)
                unit = Tools.GetUnitSymbol(Units.Value);
            DataTreeValueLabel[] labels = null;
            if ((Labels?.Length ?? 0) != 0)
                labels = Labels.Select(lb => new DataTreeValueLabel(lb.Value, (lb.DisplayName ?? lb.Name))).ToArray();

            return new DataTree(this.Name, 0, convertRawValueToFormated((T)value), issueList.Count != 0 ? issueList.ToArray() : null, unit, labels);
        }
    }
}