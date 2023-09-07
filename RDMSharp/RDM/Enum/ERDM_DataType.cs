namespace RDMSharp
{
    public enum ERDM_DataType : byte
    {
        NOT_DEFINED = 0x00,
        BIT_FIELD = 0x01,
        ASCII = 0x02,
        UNSIGNED_BYTE = 0x03,
        SIGNED_BYTE = 0x04,
        UNSIGNED_WORD = 0x05,
        SIGNED_WORD = 0x06,
        UNSIGNED_DWORD = 0x07,
        SIGNED_DWORD = 0x08
    }
}
