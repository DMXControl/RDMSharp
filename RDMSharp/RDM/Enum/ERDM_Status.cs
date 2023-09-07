﻿namespace RDMSharp
{
    public enum ERDM_Status : byte
    {
        NONE = 0x00,
        GET_LAST_MESSAGE = 0x01,
        ADVISORY = 0x02,
        WARNING = 0x03,
        ERROR = 0x04,
        ADVISORY_CLEARED = 0x12,
        WARNING_CLEARED = 0x13,
        ERROR_CLEARED = 0x14,
    }
}
