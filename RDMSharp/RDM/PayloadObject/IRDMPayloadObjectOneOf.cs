using System;

namespace RDMSharp
{
    public interface IRDMPayloadObjectOneOf : IRDMPayloadObjectIndex
    {
        Type IndexType { get; }
        object Count { get; }
        ERDM_Parameter DescriptorParameter { get; }
    }
}