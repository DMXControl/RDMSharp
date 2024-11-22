using RDMSharp.Metadata.JSON.Converter;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    [JsonConverter(typeof(CustomEnumConverter<EIntegerType>))]
    public enum EIntegerType
    {
        [JsonPropertyName("int8")]
        Int8,
        [JsonPropertyName("int16")]
        Int16,
        [JsonPropertyName("int32")]
        Int32,
        [JsonPropertyName("int64")]
        Int64,
        [JsonPropertyName("int128")]
        Int128,
        [JsonPropertyName("uint8")]
        UInt8,
        [JsonPropertyName("uint16")]
        UInt16,
        [JsonPropertyName("uint32")]
        UInt32,
        [JsonPropertyName("uint64")]
        UInt64,
        [JsonPropertyName("uint128")]
        UInt128
    }
}