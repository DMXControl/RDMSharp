using RDMSharp.Metadata.JSON.OneOfTypes;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf = RDMSharp.Metadata.JSON.OneOfTypes.OneOfTypes;

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
            }

            throw new JsonException($"Unexpected JSON format Type: {type} for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, OneOf value, JsonSerializerOptions options)
        {
            if (value.BitFieldType != null)
                JsonSerializer.Serialize(writer, value.BitFieldType, options);

            else if (value.BooleanType != null)
                JsonSerializer.Serialize(writer, value.BooleanType, options);

            else if (value.BytesType != null)
                JsonSerializer.Serialize(writer, value.BytesType, options);

            else if (value.ReferenceType != null)
                JsonSerializer.Serialize(writer, value.ReferenceType, options);

            else if (value.IntegerType_UInt8 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt8, options);
            else if (value.IntegerType_Int8 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_Int8, options);

            else if (value.IntegerType_UInt16 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt16, options);
            else if (value.IntegerType_Int16 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_Int16, options);

            else if (value.IntegerType_UInt32 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt32, options);
            else if (value.IntegerType_Int32 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_Int32, options);

            else if (value.IntegerType_UInt64 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt64, options);
            else if (value.IntegerType_Int64 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_Int64, options);
#if NET7_0_OR_GREATER
            else if (value.IntegerType_UInt128 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_UInt128, options);
            else if (value.IntegerType_Int128 != null)
                JsonSerializer.Serialize(writer, value.IntegerType_Int128, options);
#endif
            else if (value.StringType != null)
                JsonSerializer.Serialize(writer, value.StringType, options);

            else if (value.ListType != null)
                JsonSerializer.Serialize(writer, value.ListType, options);

            else if (value.CompoundType != null)
                JsonSerializer.Serialize(writer, value.CompoundType, options);

            else if (value.PD_EnvelopeType != null)
                JsonSerializer.Serialize(writer, value.PD_EnvelopeType, options);
        }
    }
}
