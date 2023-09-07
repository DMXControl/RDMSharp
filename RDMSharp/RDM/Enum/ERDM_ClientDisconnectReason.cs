namespace RDMSharp
{
    public enum ERDM_ClientDisconnectReason : ushort
    {
        SHUTDOWN = 0x0000,
        CAPACITY_EXHAUSTED = 0x0001,
        HARDWARE_FAULT = 0x0002,
        SOFTWARE_FAULT = 0x0003,
        SOFTWARE_RESET = 0x0004,
        INCORRECT_SCOPE = 0x0005,
        RPT_RECONFIGURE = 0x0006,
        LLRP_RECONFIGURE = 0x0007,
        USER_RECONFIGURE = 0x0008
    }
}
