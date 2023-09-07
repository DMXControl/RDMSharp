using System;

namespace RDMSharp
{
    [Flags]
    public enum ERDM_Command : byte
    {
        NONE = 0x00,
        RESPONSE = 0x01,

        DISCOVERY_COMMAND = 0x10,
        DISCOVERY_COMMAND_RESPONSE = DISCOVERY_COMMAND | RESPONSE,
        GET_COMMAND = 0x20,
        GET_COMMAND_RESPONSE = GET_COMMAND | RESPONSE,
        SET_COMMAND = 0x30,
        SET_COMMAND_RESPONSE = SET_COMMAND | RESPONSE,
    }
}