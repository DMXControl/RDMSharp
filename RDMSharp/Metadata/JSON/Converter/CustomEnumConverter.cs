using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.Converter
{
    public class CustomEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumValue = reader.GetString();

            foreach (var field in typeof(T).GetFields())
            {
                if (field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == enumValue)
                {
                    return (T)field.GetValue(null);
                }
            }

            throw new JsonException($"Unknown enum value: {enumValue}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var field = typeof(T).GetField(value.ToString());

            var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
            string enumString = attribute.Name;

            writer.WriteStringValue(enumString);
        }
    }

}
