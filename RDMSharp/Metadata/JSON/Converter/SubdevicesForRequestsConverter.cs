using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.Converter
{
    public class SubdevicesForRequestsConverter : JsonConverter<SubdevicesForRequests>
    {
        public override SubdevicesForRequests Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = JsonSerializer.Deserialize<SubdevicesForRequests.ESubdevicesForRequests>(ref reader, options);
                return new SubdevicesForRequests(enumValue);
            }
            else if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.Number)
            {
                var objectValue = JsonSerializer.Deserialize<SubdeviceType>(ref reader, options);
                return new SubdevicesForRequests(objectValue);
            }

            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, SubdevicesForRequests value, JsonSerializerOptions options)
        {
            if (value.EnumValue.HasValue)
                JsonSerializer.Serialize(writer, value.EnumValue.Value, new JsonSerializerOptions(options)
                {
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            else if (value.ObjectValue != null)
                JsonSerializer.Serialize(writer, value.ObjectValue, options);
        }
    }
}
