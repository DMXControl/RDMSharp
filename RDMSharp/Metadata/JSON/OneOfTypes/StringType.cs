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
        public StringType(
            string name,
            string displayName,
            string notes,
            string format,
            string pattern,
            uint length,
            bool? restrictToASCII = null) : this(
                name,
                displayName,
                notes,
                null,
                "string",
                format,
                pattern,
                minLength: length,
                maxLength: length,
                null,
                null,
                restrictToASCII)
        {
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
        public override IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            if (dataTree.Value is string @string)
            {
                if (MinLength.HasValue && MinLength.Value > @string.Length)
                    throw new ArithmeticException($"The given String is smaller then {nameof(MinLength)}: {MinLength}");
                if (MaxLength.HasValue && MaxLength.Value < @string.Length)
                    throw new ArithmeticException($"The given String is larger then {nameof(MaxLength)}: {MaxLength}");

                Encoding encoder = null;
                if (RestrictToASCII == true)
                    encoder = Encoding.ASCII;
                else
                    encoder = Encoding.UTF8;

                var data = encoder.GetBytes(@string);

                if (MinBytes.HasValue && MinBytes.Value > data.Length)
                    throw new ArithmeticException($"The given String encoded is smaller then {nameof(MinBytes)}: {MinBytes}");
                if (MaxBytes.HasValue && MaxBytes.Value < data.Length)
                    throw new ArithmeticException($"The given String encoded is larger then {nameof(MaxBytes)}: {MaxBytes}");

                return Tools.EncaseData(data);
            }

            throw new ArithmeticException($"The given Object from {nameof(dataTree.Value)} can't be parsed");
        }

        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            if (MaxBytes.HasValue && data.Length > MaxBytes.Value)
                issueList.Add(new DataTreeIssue($"Data length exceeds {nameof(MaxBytes)}, the Data has {data.Length}, but {nameof(MaxBytes)} is {MaxBytes}"));
            if (MinBytes.HasValue && data.Length < MinBytes.Value)
                issueList.Add(new DataTreeIssue($"Data length falls shorts of {nameof(MinBytes)}, the Data has {data.Length}, but {nameof(MinBytes)} is {MinBytes}"));

            string str = null;
            byte[] dataBytes = data;
            int charLength = 0;
            int byteLength = 0;
            int takenBytesCount = 0;
            if (RestrictToASCII == true)
                parse(Encoding.ASCII);
            else
                parse(Encoding.UTF8);

            void parse(Encoding encoder)
            {
                str = encoder.GetString(dataBytes);
                takenBytesCount = dataBytes.Length;

                string[] strings = str.Split((char)0);
                if (strings.Where(s => string.IsNullOrEmpty(s)).Count() > 1)
                    issueList.Add(new DataTreeIssue("More then one Null-Delimiter"));
                if (strings.Skip(1).Any(s => !string.IsNullOrEmpty(s)))
                    issueList.Add(new DataTreeIssue("Trailing Characters"));

                str = strings.First();
                byteLength = encoder.GetBytes(str).Length;
                takenBytesCount = byteLength;
                charLength = str.Length;
                bool repeatParse = false;
                if (MaxLength.HasValue && MaxLength.Value < charLength)
                {
                    charLength = (int)MaxLength.Value;
                    repeatParse = true;
                }
                if (MaxBytes.HasValue && MaxBytes.Value < byteLength)
                {
                    byteLength = (int)MaxBytes.Value;
                    repeatParse = true;
                }
                if (repeatParse)
                {
                    dataBytes = dataBytes.Take(byteLength).ToArray();
                    str = encoder.GetString(dataBytes);
                    takenBytesCount = dataBytes.Length;
                }

            }

            if (MaxLength.HasValue && str.Length > MaxLength.Value)
                issueList.Add(new DataTreeIssue($"String length exceeds {nameof(MaxLength)}, the Data has {str.Length}, but {nameof(MaxLength)} is {MaxLength}"));
            if (MinLength.HasValue && str.Length < MinLength.Value)
                issueList.Add(new DataTreeIssue($"String length falls shorts of {nameof(MinLength)}, the Data has {str.Length}, but {nameof(MinLength)} is {MinLength}"));

            data = data.Skip(takenBytesCount).ToArray();
            return new DataTree(this.Name, 0, str, issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}
