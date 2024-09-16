using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.JSON
{
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
}
