﻿using System;
using System.Linq;
using System.Text.Json.Serialization;
using RDMSharp.RDM;

namespace RDMSharp.Metadata.JSON
{
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

        public abstract byte[] ParsePayloadToData(DataTree dataTree);

        public override string ToString()
        {
            if(!string.IsNullOrWhiteSpace(DisplayName))
                return DisplayName;

            return Name;
        }
    }
}
