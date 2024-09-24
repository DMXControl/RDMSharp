using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class StringType : CommonPropertiesForNamed
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
        [JsonPropertyOrder(25)]
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
        [JsonPropertyOrder(21)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Format { get; }
        [JsonPropertyName("pattern")]
        [JsonPropertyOrder(22)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Pattern { get; }
        [JsonPropertyName("minLength")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MinLength { get; }
        [JsonPropertyName("maxLength")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MaxLength { get; }
        [JsonPropertyName("minBytes")]
        [JsonPropertyOrder(41)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MinBytes { get; }
        [JsonPropertyName("maxBytes")]
        [JsonPropertyOrder(42)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MaxBytes { get; }
        [JsonPropertyName("restrictToASCII")]
        [JsonPropertyOrder(51)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? RestrictToASCII { get; }


        [JsonConstructor]
        public StringType(string name,
                          string displayName,
                          string notes,
                          string[] resources,
                          string type,
                          string format,
                          string pattern,
                          uint? minLength,
                          uint? maxLength,
                          uint? minBytes,
                          uint? maxBytes,
                          bool? restrictToASCII)
        {
            if (!"string".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"string\"");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Format = format;
            Pattern = pattern;
            MinLength = minLength;
            MaxLength = maxLength;
            MinBytes = minBytes;
            MaxBytes = maxBytes;
            RestrictToASCII = restrictToASCII;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override PDL GetDataLength()
        {
            uint min = 0;
            uint? max = null;
            if (MinLength.HasValue)
                min = (uint)MinLength.Value;
            if (MaxLength.HasValue)
                max = (uint)MaxLength.Value;
            if (MinBytes.HasValue)
                min = (uint)MinBytes.Value;
            if (MaxBytes.HasValue)
                max = (uint)MaxBytes.Value;

            if (!max.HasValue)
                return new PDL(min, PDL.MAX_LENGTH);

            return new PDL(min, max.Value);
        }
        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            if (dataTree.Value is string @string)
            {
                if (MaxLength.HasValue)
                    @string = @string.Substring(0, (int)MaxLength);

                if (RestrictToASCII == true)
                    Encoding.ASCII.GetBytes(@string);
                else
                    Encoding.UTF8.GetBytes(@string);
            }

            throw new ArithmeticException($"The given Object from {nameof(dataTree.Value)} can't be parsed");
        }

        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            if (MaxBytes.HasValue && data.Length > MaxBytes)
                issueList.Add(new DataTreeIssue($"Data length exceeds {nameof(MaxBytes)}, the Data has {data.Length}, but {nameof(MaxBytes)} is {MaxBytes}"));
            if (MinBytes.HasValue && data.Length < MinBytes)
                issueList.Add(new DataTreeIssue($"Data length falls shorts of {nameof(MinBytes)}, the Data has {data.Length}, but {nameof(MinBytes)} is {MinBytes}"));
            
            string str = null;
            uint length = (uint)data.Length;
            if (MaxLength.HasValue)
                length = MaxLength.Value;
            if (MaxBytes.HasValue)
                length = MaxBytes.Value;

            if (data.Any(c => c == 0))
                length = (uint)data.TakeWhile(c => c != 0).Count() + 1;

            if (RestrictToASCII == true)
                str = Encoding.ASCII.GetString(data, 0, (int)length);
            else
                str = Encoding.UTF8.GetString(data, 0, (int)length);

            if (MaxLength.HasValue && str.Length > MaxLength)
                issueList.Add(new DataTreeIssue($"String length exceeds {nameof(MaxLength)}, the Data has {str.Length}, but {nameof(MaxLength)} is {MaxLength}"));
            if (MinLength.HasValue && str.Length < MinLength)
                issueList.Add(new DataTreeIssue($"String length falls shorts of {nameof(MinLength)}, the Data has {str.Length}, but {nameof(MinLength)} is {MinLength}"));

            data = data.Skip((int)length).ToArray();
            return new DataTree(this.Name, 0, str, issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}
