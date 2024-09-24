namespace RDMSharp.Metadata
{
    public readonly struct DataTree
    {
        public readonly string Name;
        public readonly uint Index;
        public readonly object? Value;
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
            Children = dataTree.Children;
        }
        public DataTree(string name, uint index, object value, DataTreeIssue[]? issues = null) : this(name, index, issues)
        {
            Value = value;
        }

        public DataTree(string name, uint index, DataTree[] children, DataTreeIssue[]? issues = null) : this(name, index, issues)
        {
            Children = children;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        } 
    }
}