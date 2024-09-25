using RDMSharp.RDM;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class LabeledIntegerType : CommonPropertiesForNamed
    {
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

        [JsonPropertyName("value")]
        [JsonPropertyOrder(3)]
        public long Value { get; }

        [JsonConstructor]
        public LabeledIntegerType(string name,
                                  string displayName,
                                  string notes,
                                  string[] resources,
                                  long value) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Value = value;
        }

        public LabeledIntegerType(string name, long value): this(name, null, null, null, value)
        {

        }

        public override string ToString()
        {
            return $"{Value} -> {Name}";
        }
        public override PDL GetDataLength()
        {
            throw new NotSupportedException();
        }

        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            throw new NotSupportedException();
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            throw new NotSupportedException();
        }
    }
}
