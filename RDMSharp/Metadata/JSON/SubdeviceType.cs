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
            if (Value.HasValue)
                return $"Subdevice Value: {Value:X4}";
            if (Range.HasValue)
                return Range.ToString();
            return base.ToString();
        }
    }
}
