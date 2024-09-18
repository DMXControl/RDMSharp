using RDMSharp.Metadata.JSON;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BitFieldType : CommonPropertiesForNamed
    {
        [JsonConstructor]
        public BitFieldType(string name,
                            string displayName,
                            string notes,
                            string[] resources,
                            string type,
                            ushort size,
                            bool? valueForUnspecified,
                            BitType[] bits) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Size = size;
            ValueForUnspecified = valueForUnspecified;
            Bits = bits;
        }

        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public override string Name { get; }
        [JsonPropertyName("displayName")]
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string DisplayName { get; }
        [JsonPropertyName("notes")]
        [JsonPropertyOrder(4)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string Notes { get; }
        [JsonPropertyName("resources")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public override string[] Resources { get; }

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public string Type { get; }

        [JsonPropertyName("size")]
        [JsonPropertyOrder(31)]
        public ushort Size { get; }

        [JsonPropertyName("valueForUnspecified")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(32)]
        public bool? ValueForUnspecified { get; }
        [JsonPropertyName("bits")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(41)]
        public BitType[] Bits { get; }

        public override string ToString()
        {
            return $"{Name} [ {string.Join("; ", Bits.Select(b => b.ToString()))} ]";
        }
    }
}
