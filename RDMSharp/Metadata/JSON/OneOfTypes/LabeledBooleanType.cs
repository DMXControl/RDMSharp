using System.Text.Json.Serialization;
using RDMSharp.RDM;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class LabeledBooleanType : CommonPropertiesForNamed
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
        public bool Value { get; }

        [JsonConstructor]
        public LabeledBooleanType(string name,
                                  string displayName,
                                  string notes,
                                  string[] resources,
                                  bool value) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Value} -> {Name}";
        }

        public override PDL GetDataLength()
        {
            throw new System.NotSupportedException();
        }
    }
}
