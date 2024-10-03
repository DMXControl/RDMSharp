using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.JSON
{
    [JsonConverter(typeof(SubdevicesForRequestsConverter))]
    public readonly struct SubdevicesForRequests
    {
        [JsonConverter(typeof(CustomEnumConverter<ESubdevicesForRequests>))]
        public enum ESubdevicesForRequests
        {
            [JsonPropertyName("root")]
            Root,
            [JsonPropertyName("subdevices")]
            Subdevices,
            [JsonPropertyName("broadcast")]
            Broadcast
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly ESubdevicesForRequests? EnumValue { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
            return EnumValue?.ToString() ?? ObjectValue.ToString();
        }
    }
}
