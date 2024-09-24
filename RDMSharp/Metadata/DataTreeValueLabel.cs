namespace RDMSharp.Metadata
{
    public readonly struct DataTreeValueLabel
    {
        public readonly object Value;
        public readonly string Label;

        public DataTreeValueLabel(object value, string label)
        {
            Value = value;
            Label = label;
        }
    }
}
