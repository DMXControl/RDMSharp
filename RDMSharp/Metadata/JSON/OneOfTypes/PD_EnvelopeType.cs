using RDMSharp.RDM;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class PD_EnvelopeType : CommonPropertiesForNamed
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

        [JsonPropertyName("length")]
        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte? Length { get; }


        [JsonConstructor]
        public PD_EnvelopeType(string name,
                               string displayName,
                               string notes,
                               string[] resources,
                               string type,
                               byte? length) : base()
        {
            if (!"pdEnvelope".Equals(type))
                throw new System.ArgumentException($"Argument {nameof(type)} has to be \"pdEnvelope\"");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Length = length;
        }

        public override string ToString()
        {
            if (Length.HasValue)
                return $"PDL: {Length} ({Length:X2}) {base.ToString()}".Trim();

            return base.ToString();
        }

        public override PDL GetDataLength()
        {
            if(Length.HasValue)
                return new PDL(Length.Value);

            return new PDL();
        }

        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            return new byte[0];
        }
    }
}
