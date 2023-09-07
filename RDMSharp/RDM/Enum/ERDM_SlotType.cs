namespace RDMSharp
{
    public enum ERDM_SlotType : byte
    {
        PRIMARY = 0x00,

        SEC_FINE = 0x01,
        SEC_TIMING = 0x02,
        SEC_SPEED = 0x03,
        SEC_CONTROL = 0x04,
        SEC_INDEX = 0x05,
        SEC_ROTATION = 0x06,
        SEC_INDEX_ROTATE = 0x07,

        SEC_UNDEFINED = 0xFF
    }
}
