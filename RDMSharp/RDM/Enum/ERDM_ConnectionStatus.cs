namespace RDMSharp
{
    public enum ERDM_ConnectionStatus : ushort
    {
        OK = 0x0000,
        SCOPE_MISMATCH = 0x0001,
        CAPACITY_EXCEEDED = 0x0002,
        DUPLICATE_UID = 0x0003,
        INVALID_CLIENT_ENTRY = 0x0004,
        INVALID_UID = 0x0005
    }
}
