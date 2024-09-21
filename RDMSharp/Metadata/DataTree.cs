namespace RDMSharp.Metadata
{
    public readonly struct DataTree
    {
        public readonly string Name;
        public readonly uint Index;
        public readonly object? Value;
        public readonly DataTree[]? Children;

        private DataTree(string name, uint index)
        {
            Name = name;
            Index = index;
        }
        public DataTree(string name, uint index, object value) : this(name, index)
        {
            Value = value;
        }

        public DataTree(string name, uint index, DataTree[] children) : this(name, index)
        {
            Children = children;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
