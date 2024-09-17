using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.OneOfTypes
{
    public class ListType
    {
        [JsonPropertyName("name")]
        public string Name { get; }
        [JsonPropertyName("type")]
        public string Type { get; }
        [JsonPropertyName("itemType")]
        public OneOfTypes ItemType { get; }
        [JsonPropertyName("minItems")]
        public int? MinItems { get; }
        [JsonPropertyName("maxItems")]
        public int? MaxItems { get; }


        [JsonConstructor]
        public ListType(
            string name,
            string type,
            OneOfTypes itemType,
            int? minItems,
            int? maxItems)
        {
            Name = name;
            Type = type;
            ItemType = itemType;
            MinItems = minItems;
            MaxItems = maxItems;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}