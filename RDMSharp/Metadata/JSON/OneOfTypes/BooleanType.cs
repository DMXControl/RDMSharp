using RDMSharp.RDM;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BooleanType : CommonPropertiesForNamed
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
        [JsonPropertyName("labels")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LabeledBooleanType[] Labels { get; }


        [JsonConstructor]
        public BooleanType(string name,
                           string displayName,
                           string notes,
                           string[] resources,
                           string type,
                           LabeledBooleanType[] labels) : base()
        {
            if (!"boolean".Equals(type))
                throw new System.ArgumentException($"Argument {nameof(type)} has to be \"boolean\"");

            if (((labels?.Length) ?? 2) != 2)
                throw new System.ArgumentException($"Argument {nameof(labels)} has to be null oa an array of 2");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Labels = labels;

            if (labels != null)
            {
                if (labels[0].Value == labels[1].Value)
                    throw new System.ArgumentException($"Argument {nameof(labels)}, both Values are the same, one has to be false, the other true");
            }
        }
        public override string ToString()
        {
            if (Labels == null)
                return Name;

            return $"{Name} [ {string.Join("; ", Labels.Select(l => l.ToString()))} ]";
        }

        public override PDL GetDataLength()
        {
            return new PDL(1);
        }
    }
}