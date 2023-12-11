using System;
using System.Linq;

namespace RDMSharp
{
    public abstract class AbstractRDMPayloadObject : IRDMPayloadObject
    {
        public abstract byte[] ToPayloadData();

        public override sealed bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;

            return obj is IRDMPayloadObject o
                && o.GetType() == this.GetType()
                && o.ToPayloadData().SequenceEqual(ToPayloadData());
        }

        public override sealed int GetHashCode()
        {
            return ToPayloadData().GenerateHashCode();
        }
    }
    public abstract class AbstractRDMPayloadObjectOneOf : AbstractRDMPayloadObject, IRDMPayloadObjectOneOf
    {
        public abstract Type IndexType { get; }
        public abstract object MinIndex { get; }
        public abstract object Index { get; }
        public abstract object Count { get; }
        public abstract ERDM_Parameter DescriptorParameter { get; }
    }
}
