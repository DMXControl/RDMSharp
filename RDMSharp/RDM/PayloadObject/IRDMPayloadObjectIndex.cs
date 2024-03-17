namespace RDMSharp
{
    public interface IRDMPayloadObjectIndex : IRDMPayloadObject
    {
        object MinIndex { get; }
        object Index { get; }
    }
}