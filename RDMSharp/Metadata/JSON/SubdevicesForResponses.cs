using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.JSON
{
    [JsonConverter(typeof(SubdevicesForResponsesConverter))]
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
}
