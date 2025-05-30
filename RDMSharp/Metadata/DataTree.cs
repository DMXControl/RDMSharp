using System;
using System.Collections.Generic;
using System.Linq;

namespace RDMSharp.Metadata
{
#pragma warning disable CS8632
    public readonly struct DataTree : IEquatable<DataTree>
    {
        public readonly string Name;
        public readonly uint Index;
        public readonly object? Value;
        public readonly string? Unit;
        public readonly bool IsCompound;
        public readonly DataTreeValueLabel[]? Labels;
        public readonly DataTree[]? Children;
        public readonly DataTreeIssue[]? Issues;

        private DataTree(string name, uint index, DataTreeIssue[]? issues = null, bool isCompound = false)
        {
            Name = name;
            Index = index;
            Issues = issues;
            IsCompound = isCompound;
        }
        public DataTree(DataTree dataTree, uint index) : this(dataTree.Name, index, dataTree.Issues, dataTree.IsCompound)
        {
            Value = dataTree.Value;
            Unit = dataTree.Unit;
            Labels = dataTree.Labels;
            Children = dataTree.Children;
        }
        public DataTree(string name, uint index, object value, DataTreeIssue[]? issues = null, string unit = null, DataTreeValueLabel[] labels = null, bool isCompound = false) : this(name, index, issues, isCompound)
        {
            if (value is IEnumerable<DataTree> || value is DataTree[] children)
                throw new ArgumentException($"Use other Constructor if you use {nameof(Children)}");

            Value = value;
            Unit = unit;
            Labels = labels;
        }

        public DataTree(string name, uint index, DataTree[] children, DataTreeIssue[]? issues = null, bool isCompound = false) : this(name, index, issues, isCompound)
        {
            Children = children;
        }

        public override string ToString()
        {
            return $"[{Index}] {Name}: {Value}";
        }

        public override bool Equals(object obj)
        {
            return obj is DataTree tree && Equals(tree);
        }

        public bool Equals(DataTree other)
        {
            return Name == other.Name &&
                   Index == other.Index &&
                   EqualityComparer<object>.Default.Equals(Value, other.Value) &&
                   compairArrays(this, other);

            bool compairArrays(DataTree _this, DataTree other)
            {
                if (_this.Children != null)
                {
                    if (!_this.Children.SequenceEqual(other.Children))
                        return false;
                }
                else if (other.Children != null)
                    return false;
                if (_this.Issues != null)
                {
                    if (!_this.Issues.SequenceEqual(other.Issues))
                        return false;
                }
                else if (other.Issues != null)
                    return false;

                return true;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Index, Value, Children, Issues);
        }

        public static bool operator ==(DataTree left, DataTree right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataTree left, DataTree right)
        {
            return !(left == right);
        }
    }
}
#pragma warning restore CS8632