namespace RDMSharp
{
    public enum ERDM_StatusMessage : ushort
    {
        CAL_FAIL = 0x0001,

        SENS_NOT_FOUND = 0x0002,
        SENS_ALWAYS_ON = 0x0003,
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        FEEDBACK_ERROR = 0x0004,
        INDEX_ERROR = 0x0005,
        #endregion

        LAMP_DOUSED = 0x0011,
        LAMP_STRIKE = 0x0012,
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        LAMP_ACCESS_OPEN = 0x0013,
        LAMP_ALWAYS_ON = 0x0014,
        #endregion

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
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        LOAD_FAILURE = 0x0046,
        #endregion

        READY = 0x0050,
        NOT_READY = 0x0051,
        LOW_FLUID = 0x0052,

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        EEPROM_ERROR = 0x0060,
        RAM_ERROR = 0x0061,
        FPGA_ERROR = 0x0062,
        #endregion

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        PROXY_BROADCAST_DROPPED = 0x0070,
        ASC_RXOK = 0x0071,
        ASC_DROPPED = 0x0072,
        #endregion

        #region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        DMXNSCNONE = 0x0080,
        DMXNSCLOSS = 0x0081,
        DMXNSCERROR = 0x0082,
        DMXNSC_OK = 0x0083,
        #endregion
    }
}
