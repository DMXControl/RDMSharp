namespace RDMSharp
{
    public enum ERDM_StatusMessage : ushort
    {
        CAL_FAIL = 0x0001,

        SENS_NOT_FOUND = 0x0002,
        SENS_ALWAYS_ON = 0x0003,

        LAMP_DOUSED = 0x0011,
        LAMP_STRIKE = 0x0012,

        OVERTEMP = 0x0021,
        UNDERTEMP = 0x0022,
        SENS_OUT_RANGE = 0x0023,

        OVERVOLTAGE_PHASE = 0x0031,
        UNDERVOLTAGE_PHASE = 0x0032,
        OVERCURRENT = 0x0033,
        UNDERCURRENT = 0x0034,
        PHASE = 0x0035,
        PHASE_ERROR = 0x0036,
        AMPS = 0x0037,
        VOLTS = 0x0038,

        DIMSLOT_OCCUPIED = 0x0041,
        BREAKER_TRIP = 0x0042,
        WATTS = 0x0043,
        DIM_FAILURE = 0x0044,
        DIM_PANIC = 0x0045,

        READY = 0x0050,
        NOT_READY = 0x0051,
        LOW_FLUID = 0x0052
    }
}
