using RDMSharp.Metadata.JSON;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class ListType : CommonPropertiesForNamed
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

        [JsonPropertyName("type")]
        [JsonPropertyOrder(3)]
        public string Type { get; }
        [JsonPropertyName("itemType")]
        [JsonPropertyOrder(21)]
        public OneOfTypes ItemType { get; }
        [JsonPropertyName("minItems")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MinItems { get; }
        [JsonPropertyName("maxItems")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxItems { get; }


        [JsonConstructor]
        public ListType(string name,
                        string displayName,
                        string notes,
                        string[] resources,
                        string type,
                        OneOfTypes itemType,
                        int? minItems,
                        int? maxItems) : base()
        {
            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            ItemType = itemType;
            MinItems = minItems;
            MaxItems = maxItems;
        }

        public override string ToString()
        {
            return DisplayName ?? Name;
        }
    }
}