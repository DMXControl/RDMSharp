using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata
{
    public readonly struct MetadataJSONObjectDefine
    {
        [JsonPropertyName("name")]
        public readonly string Name { get; }
        [JsonPropertyName("manufacturer_id")]
        public readonly ushort ManufacturerID { get; }
        [JsonPropertyName("pid")]
        public readonly ushort PID { get; }
        [JsonPropertyName("version")]
        public readonly ushort Version { get; }
        [JsonPropertyName("get_request_subdevice_range")]
        public readonly SubdevicesForRequests[] GetRequestSubdeviceRange { get; }
        [JsonPropertyName("get_response_subdevice_range")]
        public readonly SubdevicesForResponses[] GetResponseSubdeviceRange { get; }
        [JsonPropertyName("set_request_subdevice_range")]
        public readonly SubdevicesForRequests[] SetReequestsSubdeviceRange { get; }
        [JsonPropertyName("set_response_subdevice_range")]
        public readonly SubdevicesForResponses[] SetResponseSubdeviceRange { get; }

        [JsonConstructor]
        public MetadataJSONObjectDefine(string name, ushort manufacturerID, ushort pID, ushort version, SubdevicesForRequests[] getRequestSubdeviceRange, SubdevicesForResponses[] getResponseSubdeviceRange, SubdevicesForRequests[] setReequestsSubdeviceRange, SubdevicesForResponses[] setResponseSubdeviceRange)
        {
            Name = name;
            ManufacturerID = manufacturerID;
            PID = pID;
            Version = version;
            GetRequestSubdeviceRange = getRequestSubdeviceRange;
            GetResponseSubdeviceRange = getResponseSubdeviceRange;
            SetReequestsSubdeviceRange = setReequestsSubdeviceRange;
            SetResponseSubdeviceRange = setResponseSubdeviceRange;
        }

        public override string ToString()
        {
            return $"{ManufacturerID:X4} {PID:X4} {Name}";
        }
    }
    [JsonConverter(typeof(SubdevicesForRequestsConverter))]
    public readonly struct SubdevicesForRequests
    {
        public enum ESubdevicesForRequests
        {
            [JsonPropertyName("root")]
            Root,
            [JsonPropertyName("subdevices")]
            Subdevices,
            [JsonPropertyName("broadcast")]
            Broadcast
        }
        public readonly ESubdevicesForRequests? EnumValue { get; }
        public readonly SubdeviceType? ObjectValue { get; }
        public SubdevicesForRequests(ESubdevicesForRequests enumValue)
        {
            EnumValue = enumValue;
        }
        public SubdevicesForRequests(SubdeviceType objectValue)
        {
            ObjectValue = objectValue;
        }
        public override string ToString()
        {
            if (EnumValue.HasValue)
                return EnumValue.Value.ToString();
            if (ObjectValue.HasValue)
                return ObjectValue.Value.ToString();
            return base.ToString();
        }
    }
    public readonly struct SubdevicesForResponses
    {
        public enum ESubdevicesForResponses
        {
            [JsonPropertyName("root")]
            Root,
            [JsonPropertyName("subdevices")]
            Subdevices,
            [JsonPropertyName("broadcast")]
            Broadcast,
            [JsonPropertyName("match")]
            Match
        }
        public readonly ESubdevicesForResponses? EnumValue { get; }
        public readonly SubdeviceType? ObjectValue { get; }
        public SubdevicesForResponses(ESubdevicesForResponses enumValue)
        {
            EnumValue = enumValue;
        }
        public SubdevicesForResponses(SubdeviceType objectValue)
        {
            ObjectValue = objectValue;
        }
        public override string ToString()
        {
            if (EnumValue.HasValue)
                return EnumValue.Value.ToString();
            if (ObjectValue.HasValue)
                return ObjectValue.Value.ToString();
            return base.ToString();
        }
    }
    [JsonConverter(typeof(SubdeviceTypeConverter))]
    public readonly struct SubdeviceType
    {
        public readonly ushort? Value { get; }
        public readonly SubdeviceRange? Range { get; }
        public SubdeviceType(ushort value) : this()
        {
            Value = value;
        }

        public SubdeviceType(SubdeviceRange range) : this()
        {
            Range = range;
        }
        public override string ToString()
        {
            if(Value.HasValue)
                return $"Subdevice Value: {Value:X4}";
            if (Range.HasValue)
                return Range.ToString();
            return base.ToString();
        }
    }
    public readonly struct SubdeviceRange
    {
        [JsonPropertyName("minimum")]
        public readonly ushort Minimum { get; }

        [JsonPropertyName("maximum")]
        public readonly ushort Maximum { get; }

        [JsonConstructor]
        public SubdeviceRange(ushort minimum, ushort maximum) : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }
        public override string ToString()
        {
            return $"Subdevice Range: {Minimum:X4} - {Maximum:X4}";
        }
    }
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
    public class SubdevicesForRequestsConverter : JsonConverter<SubdevicesForRequests>
    {
        public override SubdevicesForRequests Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var _options = new JsonSerializerOptions(options)
                {
                    Converters = { new JsonStringEnumConverter() },
                    PropertyNameCaseInsensitive = true
                };
                var enumValue = JsonSerializer.Deserialize<SubdevicesForRequests.ESubdevicesForRequests>(ref reader, _options);
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
                JsonSerializer.Serialize(writer, value.EnumValue.Value, options);
            else if (value.ObjectValue != null)
                JsonSerializer.Serialize(writer, value.ObjectValue, options);
        }
    }
    public class SubdevicesForReponseConverter : JsonConverter<SubdevicesForResponses>
    {
        public override SubdevicesForResponses Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var _options = new JsonSerializerOptions(options)
                {
                    Converters = { new JsonStringEnumConverter() },
                    PropertyNameCaseInsensitive = true
                };
                var enumValue = JsonSerializer.Deserialize<SubdevicesForResponses.ESubdevicesForResponses>(ref reader, _options);
                return new SubdevicesForResponses(enumValue);
            }
            else if (reader.TokenType == JsonTokenType.StartObject || reader.TokenType == JsonTokenType.Number)
            {
                var objectValue = JsonSerializer.Deserialize<SubdeviceType>(ref reader, options);
                return new SubdevicesForResponses(objectValue);
            }

            throw new JsonException("Unexpected JSON format for FieldContainer.");
        }

        public override void Write(Utf8JsonWriter writer, SubdevicesForResponses value, JsonSerializerOptions options)
        {
            if (value.EnumValue.HasValue)
                JsonSerializer.Serialize(writer, value.EnumValue.Value, options);
            else if (value.ObjectValue != null)
                JsonSerializer.Serialize(writer, value.ObjectValue, options);
        }
    }
}
