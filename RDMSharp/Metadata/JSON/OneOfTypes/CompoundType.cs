using RDMSharp.RDM;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class CompoundType : CommonPropertiesForNamed
    {
        [JsonPropertyName("name")]
        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
        [JsonPropertyName("subtypes")]
        [JsonPropertyOrder(11)]
        public OneOfTypes[] Subtypes { get; }


        [JsonConstructor]
        public CompoundType(string name,
                            string displayName,
                            string notes,
                            string[] resources,
                            string type,
                            OneOfTypes[] subtypes)
        {
            if (!"compound".Equals(type))
                throw new System.ArgumentException($"Argument {nameof(type)} has to be \"compound\"");

            if (((subtypes?.Length) ?? 0) < 1)
                throw new System.ArgumentException($"Argument {nameof(subtypes)} has to be at least a size of 1");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Subtypes = subtypes;
        }

        public override PDL GetDataLength()
        {
            return new PDL(Subtypes.Select(s => s.GetDataLength()).ToArray());
        }
    }
}