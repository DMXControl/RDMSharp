using System.Text.Json.Serialization;
using RDMSharp.Metadata.JSON.Converter;

namespace RDMSharp.Metadata.JSON
{
    [JsonConverter(typeof(SubdeviceTypeConverter))]
    public readonly struct SubdeviceType
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public readonly ushort? Value { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
            return Value?.ToString() ?? Range.ToString();
        }
    }
}
