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
                case "boolean":
                    var booleanType = element.Deserialize<BooleanType>(options);
                    return new OneOf(booleanType);

                case "compound":
                    break; // ToDo

                case "int8":
                case "int16":
                case "int32":
                case "int64":
                case "int128":
                case "uint8":
                case "uint16":
                case "uint32":
                case "uint64":
                case "uint128":
                    var integerType = element.Deserialize<IntegerType>(options);
                    return new OneOf(integerType);

                case "list":
                    break; // ToDo

                case "pdEnvelope":
                    break; // ToDo

                case "string":
                    var stringType = element.Deserialize<StringType>(options);
                    return new OneOf(stringType);


                case null:
                default:
                    break;
            }

            return new OneOf();
            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, OneOf value, JsonSerializerOptions options)
        {
            if (value.BitType.HasValue)
                JsonSerializer.Serialize(writer, value.BitType.Value, options);
            else if (value.BitFieldType != null)
                JsonSerializer.Serialize(writer, value.BitFieldType.Value, options);
        }
    }
}
