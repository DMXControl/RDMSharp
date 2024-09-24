using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BitFieldType : CommonPropertiesForNamed
    {
        [JsonConstructor]
        public BitFieldType(string name,
                            string displayName,
                            string notes,
                            string[] resources,
                            string type,
                            ushort size,
                            bool? valueForUnspecified,
                            BitType[] bits) : base()
        {
            if (!"bitField".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"bitField\"");
            if (size % 8 != 0)
                throw new ArgumentOutOfRangeException($"Argument {nameof(size)} has to be a multiple of 8");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            Size = size;
            ValueForUnspecified = valueForUnspecified;
            Bits = bits;
        }

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

        [JsonPropertyName("size")]
        [JsonPropertyOrder(31)]
        public ushort Size { get; }

        [JsonPropertyName("valueForUnspecified")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(32)]
        public bool? ValueForUnspecified { get; }
        [JsonPropertyName("bits")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(41)]
        public BitType[] Bits { get; }

        public override PDL GetDataLength()
        {
            return new PDL((uint)(Size / 8));
        }

        public override string ToString()
        {
            return $"{Name} [ {string.Join("; ", Bits.Select(b => b.ToString()))} ]";
        }
        public override byte[] ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");
            if (dataTree.Children.Length != this.Bits.Length)
                throw new ArithmeticException($"The given {nameof(dataTree.Children)}.{nameof(dataTree.Children.Length)}({dataTree.Children.Length}) not match {nameof(Bits)}.{nameof(Bits.Length)}({Bits.Length})");

            bool[] data = new bool[Size];
            if (ValueForUnspecified.HasValue)
                for (int i = 0; i < Size; i++)
                    data[i] = ValueForUnspecified.Value;

            foreach (DataTree bitDataTree in dataTree.Children)
            {
                BitType bit = Bits.FirstOrDefault(b=>b.Name== bitDataTree.Name);
                if (bit == null)
                    throw new ArithmeticException($"Can't find matching BitType {bitDataTree.Name}");
                if (bitDataTree.Index != bit.Index)
                    throw new ArithmeticException($"The given DataTree {nameof(bitDataTree.Index)}({bitDataTree.Index}) not match BitType {nameof(bit.Index)}({bit.Index})");
                if (bitDataTree.Value is not bool value)
                    throw new ArithmeticException($"DataTree VAlue is not bool");

                data[bit.Index] = value;
            }

            return Tools.ValueToData(data);
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTree> bitDataTrees = new List<DataTree>();
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            int byteCount = (Size / 8);
            if (byteCount != data.Length)
                issueList.Add(new DataTreeIssue($"Data length not match given Size/8 ({byteCount})"));
           
            bool[] bools = Tools.DataToBoolArray(ref data, this.Size);
            foreach (BitType bitType in Bits)
                bitDataTrees.Add(new DataTree(bitType.Name, bitType.Index, bools[bitType.Index]));

            data = data.Skip(byteCount).ToArray();

            return new DataTree(this.Name, 0, bitDataTrees.OrderBy(b=>b.Index), issueList.Count != 0 ? issueList.ToArray() : null);
        }
    }
}
