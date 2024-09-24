using RDMSharp.RDM;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Drawing;

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
                throw new ArgumentException($"Argument {nameof(type)} has to be \"compound\"");

            if (((subtypes?.Length) ?? 0) < 1)
                throw new ArgumentException($"Argument {nameof(subtypes)} has to be at least a size of 1");

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
        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            if(dataTree.Children.Length!= Subtypes.Length)
                throw new ArithmeticException($"The given {nameof(dataTree)} and {nameof(Subtypes)} has different length ");

            List<byte> data = new List<byte>();
            for (int i = 0; i < dataTree.Children.Length; i++)
            {
                if (Subtypes[i].IsEmpty())
                    throw new ArithmeticException($"The given Object from {nameof(Subtypes)}[{i}] is Empty");

                data.AddRange(Subtypes[i].ParsePayloadToData(dataTree.Children[i]));
            }

            if (!GetDataLength().IsValid(data.Count))
                throw new ArithmeticException($"Parsed DataLengt not fits Calculated DataLength");

            return data.ToArray();
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTree> subTypeDataTree = new List<DataTree>();
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();

            for (int i = 0; i < Subtypes.Length; i++)
            {
                OneOfTypes subType = Subtypes[i];
                subTypeDataTree.Add(new DataTree(subType.ParseDataToPayload(ref data), (uint)i));
            }

            return new DataTree(this.Name, 0, subTypeDataTree.OrderBy(b => b.Index), issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}