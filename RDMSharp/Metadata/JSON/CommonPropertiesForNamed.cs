using RDMSharp.RDM;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON
{
#pragma warning disable CS8632
    public abstract class CommonPropertiesForNamed
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public abstract string Name { get; }
        [JsonPropertyName("displayName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public abstract string DisplayName { get; }
        [JsonPropertyName("notes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public abstract string? Notes { get; }

        [JsonPropertyName("resources")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public abstract string[]? Resources { get; }

        public abstract PDL GetDataLength();

        public abstract IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree);
        public abstract DataTree ParseDataToPayload(ref byte[] data);

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(DisplayName))
                return DisplayName;

            return Name;
        }
    }
}
#pragma warning restore CS8632