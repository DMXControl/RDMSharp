﻿namespace RDMSharp
{
    public enum ERDM_NackReason : ushort
    {
        UNKNOWN_PID = 0x0000,
        FORMAT_ERROR = 0x0001,
        HARDWARE_FAULT = 0x0002,
        PROXY_REJECT = 0x0003,
        WRITE_PROTECT = 0x0004,
        UNSUPPORTED_COMMAND_CLASS = 0x0005,
        DATA_OUT_OF_RANGE = 0x0006,
        BUFFER_FULL = 0x0007,
        PACKET_SIZE_UNSUPPORTED = 0x0008,
        SUB_DEVICE_OUT_OF_RANGE = 0x0009,
        PROXY_BUFFER_FULL = 0x000A,
        //E1.37-2
        ACTION_NOT_SUPPORTED = 0x000B,
        //E1.33
        UNKNOWN_SCOPE = 0x000F,
        INVALID_STATIC_CONFIG_TYPE = 0x0010,
        INVALID_IPV4_ADDRESS = 0x0011,
        INVALID_IPV6_ADDRESS = 0x0012,
        INVALID_PORT = 0x0013,
        //E1.37-7
        ENDPOINT_NUMBER_INVALID = 0x000C,
        INVALID_ENDPOINT_MODE = 0x000D,
        UNKNOWN_UID = 0x000E
    }
}