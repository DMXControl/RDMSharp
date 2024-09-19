using RDMSharp.RDM;
using System;
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
            if (!"bitField".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"bitField\"");
            if (size % 8 != 0)
                throw new ArgumentOutOfRangeException($"Argument {nameof(size)} has to be a multiple of 8");

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

        public override PDL GetDataLength()
        {
            return new PDL((uint)(Size / 8));
        }

        public override string ToString()
        {
            return $"{Name} [ {string.Join("; ", Bits.Select(b => b.ToString()))} ]";
        }
    }
}
