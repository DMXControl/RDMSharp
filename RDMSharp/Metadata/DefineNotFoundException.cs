using System;

namespace RDMSharp.Metadata
{
    public class DefineNotFoundException : Exception
    {
        public DefineNotFoundException()
        {
        }

        public DefineNotFoundException(string message) : base(message)
        {
        }
    }
}
