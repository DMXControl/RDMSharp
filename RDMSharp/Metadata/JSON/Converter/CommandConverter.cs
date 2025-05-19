using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf = RDMSharp.Metadata.JSON.OneOfTypes.OneOfTypes;

namespace RDMSharp.Metadata.JSON.Converter
{
    public class CommandConverter : JsonConverter<Command>
    {
        public override Command Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = JsonSerializer.Deserialize<Command.ECommandDublicte>(ref reader, options);
                return new Command(enumValue);
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var singleField = JsonSerializer.Deserialize<OneOf>(ref reader, options);
                return new Command(singleField);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var listOfFields = JsonSerializer.Deserialize<OneOf[]>(ref reader, options);
                if (listOfFields.Length == 0)
                    return new Command();
                return new Command(listOfFields);
            }

            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, Command value, JsonSerializerOptions options)
        {
            if (value.GetIsEmpty())
                JsonSerializer.Serialize(writer, new object[0], options);
            else if (value.EnumValue.HasValue)
                JsonSerializer.Serialize(writer, value.EnumValue.Value, options);
            else if (value.SingleField != null)
                JsonSerializer.Serialize(writer, value.SingleField.Value, options);
            else if (value.ListOfFields != null)
                JsonSerializer.Serialize(writer, value.ListOfFields, options);
        }
    }
}
