using System;

namespace RDMSharp.PayloadObject;

public interface IRDMPayloadObjectOneOf : IRDMPayloadObjectIndex
{
    Type IndexType { get; }
    object Count { get; }
    ERDM_Parameter DescriptorParameter { get; }
}