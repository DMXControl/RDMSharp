namespace RDMSharp
{
    public enum ERDM_PowerState : byte
    {
        FULL_OFF = 0x00,
        SHUTDOWN = 0x01,
        STANDBY = 0x02,
        NORMAL = 0xFF
    }
}
