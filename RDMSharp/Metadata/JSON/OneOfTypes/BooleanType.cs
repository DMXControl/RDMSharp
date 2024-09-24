using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                throw new ArgumentException($"Argument {nameof(type)} has to be \"boolean\"");

            if (((labels?.Length) ?? 2) != 2)
                throw new ArgumentException($"Argument {nameof(labels)} has to be null oa an array of 2");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Labels = labels;

            if (labels != null)
            {
                if (labels[0].Value == labels[1].Value)
                    throw new ArgumentException($"Argument {nameof(labels)}, both Values are the same, one has to be false, the other true");
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

        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");
            if(dataTree.Value is bool value)
            {
                switch (value)
                {
                    case false:
                        return new byte[] { 0x00 };
                    case true:
                        return new byte[] { 0x01 };
                }
            }
            throw new ArithmeticException($"The given Object from {nameof(dataTree.Value)} can't be parsed");
        }

        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            if (data.Length != 1)
                issueList.Add(new DataTreeIssue($"Data length is not 1"));

            data = data.Skip(1).ToArray();
            return new DataTree(this.Name, 0, Tools.DataToBool(ref data), issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}