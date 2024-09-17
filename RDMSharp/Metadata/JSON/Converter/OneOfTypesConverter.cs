using RDMSharp.Metadata.OneOfTypes;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf = RDMSharp.Metadata.OneOfTypes.OneOfTypes;

namespace RDMSharp.Metadata.JSON.Converter
{
    public class OneOfTypesConverter : JsonConverter<OneOf>
    {
        public override OneOf Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonElement element = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            if (!element.TryGetProperty("type", out JsonElement typeProperty))
            {
                if (element.TryGetProperty("$ref", out JsonElement refProperty))
                {
                    var referenceType = element.Deserialize<ReferenceType>(options);
                    return new OneOf(referenceType);
                }
                throw new JsonException("Unexpected JSON format for FieldContainer.");
            }

            string type= typeProperty.GetString();
            switch (type)
            {
                case "bit":
                    var bitType = element.Deserialize<BitType>(options);
                    return new OneOf(bitType);

                case "bitField":
                    var bitFieldType = element.Deserialize<BitFieldType>(options);
                    return new OneOf(bitFieldType);

                case "bytes":
                    var bytesType = element.Deserialize<BytesType>(options);
                    return new OneOf(bytesType);

                case "boolean":
                    var booleanType = element.Deserialize<BooleanType>(options);
                    return new OneOf(booleanType);

                case "compound":
                    var compoundType = element.Deserialize<CompoundType>(options);
                    return new OneOf(compoundType);

                case "int8":
                    var integerTypeInt8 = element.Deserialize<IntegerType<sbyte>>(options);
                    return new OneOf(integerTypeInt8);
                case "uint8":
                    var integerTypeUInt8 = element.Deserialize<IntegerType<byte>>(options);
                    return new OneOf(integerTypeUInt8);

                case "int16":
                    var integerTypeInt16 = element.Deserialize<IntegerType<Int16>>(options);
                    return new OneOf(integerTypeInt16);
                case "uint16":
                    var integerTypeUInt16 = element.Deserialize<IntegerType<UInt16>>(options);
                    return new OneOf(integerTypeUInt16);

                case "int32":
                    var integerTypeInt32 = element.Deserialize<IntegerType<Int32>>(options);
                    return new OneOf(integerTypeInt32);
                case "uint32":
                    var integerTypeUInt32 = element.Deserialize<IntegerType<UInt32>>(options);
                    return new OneOf(integerTypeUInt32);

                case "int64":
                    var integerTypeInt64 = element.Deserialize<IntegerType<Int64>>(options);
                    return new OneOf(integerTypeInt64);
                case "uint64":
                    var integerTypeUInt64 = element.Deserialize<IntegerType<UInt64>>(options);
                    return new OneOf(integerTypeUInt64);

#if NET7_0_OR_GREATER
                case "int128":
                    var integerTypeInt128 = element.Deserialize<IntegerType<Int128>>(options);
                    return new OneOf(integerTypeInt128);
                case "uint128":
                    var integerTypeUInt128 = element.Deserialize<IntegerType<UInt128>>(options);
                    return new OneOf(integerTypeUInt128);
#endif

                case "list":
                    var listType = element.Deserialize<ListType>(options);
                    return new OneOf(listType);

                case "pdEnvelope":
                    var pdEnvelopeType = element.Deserialize<PD_EnvelopeType>(options);
                    return new OneOf(pdEnvelopeType);

                case "string":
                    var stringType = element.Deserialize<StringType>(options);
                    return new OneOf(stringType);


                case null:
                default:
                    break;
            }

            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, OneOf value, JsonSerializerOptions options)
        {
            if (value.BitType.HasValue)
                JsonSerializer.Serialize(writer, value.BitType.Value, options);

            else if (value.BitFieldType != null)
                JsonSerializer.Serialize(writer, value.BitFieldType.Value, options);

            else if (value.BooleanType != null)
                JsonSerializer.Serialize(writer, value.BooleanType.Value, options);

            else if (value.ReferenceType != null)
                JsonSerializer.Serialize(writer, value.ReferenceType.Value, options);

            else if (value.IntegerType_UInt8.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt8.Value, options);
            else if (value.IntegerType_Int8.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_Int8.Value, options);

            else if (value.IntegerType_UInt16.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt16.Value, options);
            else if (value.IntegerType_Int16.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_Int16.Value, options);

            else if (value.IntegerType_UInt32.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt32.Value, options);
            else if (value.IntegerType_Int32.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_Int32.Value, options);

            else if (value.IntegerType_UInt64.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt64.Value, options);
            else if (value.IntegerType_Int64.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_Int64.Value, options);
#if NET7_0_OR_GREATER
            else if (value.IntegerType_UInt128.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt128.Value, options);
            else if (value.IntegerType_Int128.HasValue)
                JsonSerializer.Serialize(writer, value.IntegerType_Int128.Value, options);
#endif
            else if (value.StringType != null)
                JsonSerializer.Serialize(writer, value.StringType.Value, options);

            else if (value.ListType != null)
                JsonSerializer.Serialize(writer, value.ListType, options);

            else if (value.CompoundType != null)
                JsonSerializer.Serialize(writer, value.CompoundType, options);

            else if (value.PD_EnvelopeType.HasValue)
                JsonSerializer.Serialize(writer, value.PD_EnvelopeType.Value, options);
            else
                throw new NotImplementedException();
        }
    }
}
