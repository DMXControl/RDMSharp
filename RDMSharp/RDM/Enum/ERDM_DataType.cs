namespace RDMSharp
{
    public enum ERDM_DataType : byte
    {
        NOT_DEFINED = 0x00,
        BIT_FIELD = 0x01,
        STRING = 0x02,
        UINT8 = 0x03,
        INT8 = 0x04,
        UINT16 = 0x05,
        INT16 = 0x06,
        UINT32 = 0x07,
        INT32 = 0x08,
        UINT64 = 0x09,
        INT64 = 0x0A,
        GROUP = 0x0B,
        UID = 0x0C,
        BOOLEAN = 0x0D,
        URL = 0x0E,
        MAC = 0x0F,
        IPv4 = 0x10,
        IPv6 = 0x11,
        ENUMERATION = 0x12
    }
}
