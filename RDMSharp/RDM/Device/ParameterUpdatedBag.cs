using System;

namespace RDMSharp
{
    public readonly struct ParameterUpdatedBag
    {
        public readonly ERDM_Parameter Parameter;
        public readonly object Index;
        public readonly DateTime Timestamp;
        public ParameterUpdatedBag(in ERDM_Parameter parameter, in object index)
        {
            Parameter = parameter;
            Index = index;
            Timestamp = DateTime.UtcNow;
        }
        public override string ToString()
        {
            if (Index == null)
                return $"{Parameter} [{Timestamp}]";

            return $"{Parameter} ({Index}) [{Timestamp}]";
        }
    }
}