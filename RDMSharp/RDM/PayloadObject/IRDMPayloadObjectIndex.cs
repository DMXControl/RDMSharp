namespace RDMSharp.PayloadObject;

public interface IRDMPayloadObjectIndex : IRDMPayloadObject
{
    object MinIndex { get; }
    object Index { get; }
}