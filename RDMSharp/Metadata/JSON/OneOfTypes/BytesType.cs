using RDMSharp.RDM;
using System;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BytesType : CommonPropertiesForNamed
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
        [JsonPropertyName("format")]
        [JsonPropertyOrder(11)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Format { get; }
        [JsonPropertyName("minLength")]
        [JsonPropertyOrder(12)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MinLength { get; }
        [JsonPropertyName("maxLength")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MaxLength { get; }


        [JsonConstructor]
        public BytesType(string name,
                         string displayName,
                         string notes,
                         string[] resources,
                         string type,
                         string format,
                         uint? minLength,
                         uint? maxLength) : base()
        {
            if (!"bytes".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"bytes\"");
            if (minLength.HasValue && maxLength.HasValue)
                if (minLength > maxLength)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(minLength)} has to be <= {nameof(maxLength)}");
            if (minLength.HasValue)
                if (minLength > PDL.MAX_LENGTH)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(minLength)} has to be <= {PDL.MAX_LENGTH}");
            if (maxLength.HasValue)
                if (maxLength > PDL.MAX_LENGTH)
                    throw new ArgumentOutOfRangeException($"Argument {nameof(maxLength)} has to be <= {PDL.MAX_LENGTH}");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Format = format;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public override string ToString()
        {
            return Name;
        }

        public override PDL GetDataLength()
        {
            return new PDL((uint)(MinLength ?? 1), (uint)(MaxLength ?? PDL.MAX_LENGTH));
        }
    }
}
