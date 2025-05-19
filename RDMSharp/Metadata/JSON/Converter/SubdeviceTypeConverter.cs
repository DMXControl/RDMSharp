using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.Converter
{
    public class SubdeviceTypeConverter : JsonConverter<SubdeviceType>
    {
        public override SubdeviceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var number = JsonSerializer.Deserialize<ushort>(ref reader, options);
                return new SubdeviceType(number);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var objectValue = JsonSerializer.Deserialize<SubdeviceRange>(ref reader, options);
                return new SubdeviceType(objectValue);
            }

            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, SubdeviceType value, JsonSerializerOptions options)
        {
            if (value.Value.HasValue)
                JsonSerializer.Serialize(writer, value.Value.Value, options);
            else if (value.Range.HasValue)
                JsonSerializer.Serialize(writer, value.Range.Value, options);
        }
    }
}
