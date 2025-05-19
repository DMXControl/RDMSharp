using System;

namespace RDMSharp.Metadata
{
    public readonly struct DataTreeIssue
    {
        public readonly string Description;

        public DataTreeIssue(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException($"{nameof(description)} has to be a vaild String");
            Description = description;
        }
        public override string ToString()
        {
            return Description;
        }
    }
}
