namespace RDMSharp
{
    public enum ERDM_LampState : byte
    {
        OFF = 0x00,
        ON = 0x01,
        STRIKE = 0x02,
        STANDBY = 0x03,
        NOT_PRESENT = 0x04,
        ERROR = 0x7F,
        //0x80 - 0xDF Manufacturer Specific
    }
}
