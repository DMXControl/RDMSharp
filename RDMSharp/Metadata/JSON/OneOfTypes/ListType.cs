﻿using RDMSharp.RDM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("RDMSharpTests")]

namespace RDMSharp.Metadata.JSON.OneOfTypes
{
    public class ListType : CommonPropertiesForNamed
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
        [JsonPropertyName("itemType")]
        [JsonPropertyOrder(21)]
        public OneOfTypes ItemType { get; }
        [JsonPropertyName("minItems")]
        [JsonPropertyOrder(31)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MinItems { get; }
        [JsonPropertyName("maxItems")]
        [JsonPropertyOrder(32)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? MaxItems { get; }


        [JsonConstructor]
        public ListType(string name,
                        string displayName,
                        string notes,
                        string[] resources,
                        string type,
                        OneOfTypes itemType,
                        uint? minItems,
                        uint? maxItems) : base()
        {
            if (!"list".Equals(type))
                throw new ArgumentException($"Argument {nameof(type)} has to be \"list\"");

            if (itemType.IsEmpty())
                throw new ArgumentException($"Argument {nameof(itemType)} is Empty, this is not allowed");

            Name = name;
            DisplayName = displayName;
            Notes = notes;
            Resources = resources;
            Type = type;
            ItemType = itemType;
            MinItems = minItems;
            MaxItems = maxItems;
        }

        public override string ToString()
        {
            return DisplayName ?? Name;
        }

        public override PDL GetDataLength()
        {
            uint min = 0;
            uint max = 0;

            if (MinItems.HasValue)
                min = MinItems.Value;
            if (MaxItems.HasValue)
                max = MaxItems.Value;

            PDL itemPDL = ItemType.GetDataLength();
            if (itemPDL.Value.HasValue)
            {
                min *= itemPDL.Value.Value;
                max *= itemPDL.Value.Value;
            }
            else
            {
                if (itemPDL.MinLength.HasValue)
                    min *= itemPDL.MinLength.Value;
                if (itemPDL.MaxLength.HasValue)
                    max *= itemPDL.MaxLength.Value;
            }

            if (max == 0)
                if (!MaxItems.HasValue)
                    max = PDL.MAX_LENGTH;

            if (min == max)
                return new PDL(min);

            return new PDL(min, max);
        }
        public override IEnumerable<byte[]> ParsePayloadToData(DataTree dataTree)
        {
            if (!string.Equals(dataTree.Name, this.Name))
                throw new ArithmeticException($"The given Name from {nameof(dataTree.Name)}({dataTree.Name}) not match this Name({this.Name})");

            List<byte[]> data = new List<byte[]>();
            for (int i = 0; i < dataTree.Children.Length; i++)
                data.AddRange(ItemType.ParsePayloadToData(dataTree.Children[i]));

            validateDataLength(data.SelectMany(en=>en).Count());

            return data.ToArray();
        }

        public override DataTree ParseDataToPayload(ref byte[] data)
        {
            List<DataTree> dataTrees = new List<DataTree>();
            List<DataTreeIssue> issueList = new List<DataTreeIssue>();

            uint index = 0;
            int dataLength = data.Length;
            while (_continue(ref data))
            {
                dataTrees.Add(new DataTree(ItemType.ParseDataToPayload(ref data), index));
                index++;
            }
            dataLength -= data.Length;

            try
            {
                validateDataLength(dataLength);
            }
            catch (Exception e)
            {
                issueList.Add(new DataTreeIssue(e.Message));
            }

            return new DataTree(Name, 0, children: dataTrees.ToArray(), issueList.Count != 0 ? issueList.ToArray() : null);

            bool _continue(ref byte[] data)
            {
                if (MaxItems.HasValue)
                    if (dataTrees.Count > MaxItems.Value)
                    {
                        if (dataTrees.Count > 0)
                            issueList.Add(new DataTreeIssue($"Given data exceeds {nameof(MaxItems)}"));
                        return false;
                    }
                if (data.Length == 0)
                {
                    if (MinItems.HasValue)
                        if (dataTrees.Count < MinItems.Value)
                            issueList.Add(new DataTreeIssue($"Given data falls shorts of {nameof(MinItems)}"));
                    return false;
                }

                return true;
            }
        }
        internal bool validateDataLength(int dataLength)
        {
            if (!GetDataLength().IsValid(dataLength))
                throw new ArithmeticException($"Parsed DataLength not fits Calculated DataLength");

            return true;
        }
    }
}