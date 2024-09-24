using System;
using System.Collections.Generic;

namespace RDMSharp.Metadata
{
    public readonly struct DataTree : IEquatable<DataTree>
    {
        public readonly string Name;
        public readonly uint Index;
        public readonly object? Value;
        public readonly string? Unit;
        public readonly DataTreeValueLabel[]? Labels;
        public readonly DataTree[]? Children;
        public readonly DataTreeIssue[]? Issues;

        private DataTree(string name, uint index, DataTreeIssue[]? issues = null)
        {
            Name = name;
            Index = index;
            Issues = issues;
        }
        public DataTree(DataTree dataTree, uint index) : this(dataTree.Name, index, dataTree.Issues)
        {
            Value = dataTree.Value;
            Unit = dataTree.Unit;
            Labels = dataTree.Labels;
            Children = dataTree.Children;
        }
        public DataTree(string name, uint index, object value, DataTreeIssue[]? issues = null, string unit = null, DataTreeValueLabel[] labels = null) : this(name, index, issues)
        {
            Value = value;
            Unit = unit;
            Labels = labels;
        }

        public DataTree(string name, uint index, DataTree[] children, DataTreeIssue[]? issues = null) : this(name, index, issues)
        {
            Children = children;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
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
                   EqualityComparer<DataTree[]>.Default.Equals(Children, other.Children) &&
                   EqualityComparer<DataTreeIssue[]>.Default.Equals(Issues, other.Issues);
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