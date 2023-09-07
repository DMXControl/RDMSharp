namespace RDMSharp
{
    public enum ERDM_DynamicUidStatus : ushort
    {
        OK = 0x0000,
        INVALID_REQUEST = 0x0001,
        UID_NOT_FOUND = 0x0002,
        DUPLICATE_UID = 0x0003,
        CAPACITY_EXHAUSTED = 0x0004
    }
}
