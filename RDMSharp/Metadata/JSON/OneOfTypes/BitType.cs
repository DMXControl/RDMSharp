using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BitType : CommonPropertiesForNamed
    {
        [JsonConstructor]
        public BitType(string name,
                       string displayName,
                       string notes,
                       string[] resources,
                       string type,
                       ushort index,
                       bool? reserved,
                       bool? valueIfReserved) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Index = index;
            Reserved = reserved;
            ValueIfReserved = valueIfReserved;
        }
        public BitType(string name, ushort index) : this(name, null, null, null, "bit", index, null, null)
        {
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; }
        [JsonPropertyName("index")]
        [JsonPropertyOrder(21)]
        public ushort Index { get; }
        [JsonPropertyName("reserved")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Reserved { get; }
        [JsonPropertyName("valueIfReserved")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ValueIfReserved { get; }

        public override PDL GetDataLength()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
        {
            throw new NotSupportedException();
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return $"{Index} -> {Name}";
        }
    }
}
