using RDMSharp.RDM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class BitFieldType : CommonPropertiesForNamed
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
        public BitFieldType(string name, ushort size, BitType[] bits, bool valueForUnspecified = false) : this(name, null, null, null, "bitField", size, valueForUnspecified, bits)
        {
        }

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
            if (ValueForUnspecified == true)
                for (int i = 0; i < Size; i++)
                    data[i] = true;

            foreach (DataTree bitDataTree in dataTree.Children)
            {
                BitType bit = Bits.FirstOrDefault(b => b.Name == bitDataTree.Name);
                if (bit == null)
                    throw new ArithmeticException($"Can't find matching BitType {bitDataTree.Name}");
                if (Bits.Length <= bitDataTree.Index || Bits[bitDataTree.Index] != bit)
                    throw new ArithmeticException($"The given DataTree {nameof(bitDataTree.Index)}({bitDataTree.Index}) not match BitType {nameof(bit.Index)}({bit.Index})");
                if (bitDataTree.Value is not bool value)
                    throw new ArithmeticException($"DataTree Value is not bool");

                data[bit.Index] = value;
            }

            return Tools.ValueToData(data);
        }
        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTree> bitDataTrees = new List<DataTree>();
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();
            int byteCount = (Size / 8);
            if (byteCount > data.Length)
            {
                issueList.Add(new DataTreeIssue($"Data length not match given Size/8 ({byteCount})"));
                byte[] cloneData = new byte[byteCount];
                Array.Copy(data, cloneData, data.Length);
                data = cloneData;
            }
            bool[] bools = Tools.DataToBoolArray(ref data, Size);
            for (uint i = 0; i < Bits.Length; i++)
            {
                BitType bitType = Bits[i];
                bitDataTrees.Add(new DataTree(bitType.Name, i, bools[bitType.Index]));
            }
            bool valueForUnspecified = ValueForUnspecified == true;
            for (int i = 0; i < bools.Length; i++)
            {
                if (Bits.Any(b => b.Index == i))
                    continue;

                bool bit = bools[i];
                if (bit != valueForUnspecified)
                    issueList.Add(new DataTreeIssue($"The Bit at Index {i} is Unspecified, but the Value is not {valueForUnspecified} as defined for Unspecified Bits"));
            }
            var children = bitDataTrees.OrderBy(b => b.Index).ToArray();
            var dataTree = new DataTree(this.Name, 0, children: children, issueList.Count != 0 ? issueList.ToArray() : null);
            return dataTree;
        }
    }
}