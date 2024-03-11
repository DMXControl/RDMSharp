using System.ComponentModel;

namespace RDMSharp
{
    public enum ERDM_NackReason : ushort
    {
        [Description("The responder cannot comply with request because the message is not implemented in responder.")]
        UNKNOWN_PID = 0x0000,
        [Description("The responder cannot interpret request as controller data was not formatted correctly.")]
        FORMAT_ERROR = 0x0001,
        [Description("The responder cannot comply due to an internal hardware fault.")]
        HARDWARE_FAULT = 0x0002,
        [Description("Proxy is not the RDM line master and cannot comply with message.")]
        PROXY_REJECT = 0x0003,
        [Description("SET Command normally allowed but being blocked currently.")]
        WRITE_PROTECT = 0x0004,
        [Description("Not valid for Command Class attempted. May be used where GET allowed but SET is not supported.")]
        UNSUPPORTED_COMMAND_CLASS = 0x0005,
        [Description("Value for given Parameter out of allowable range or not supported.")]
        DATA_OUT_OF_RANGE = 0x0006,
        [Description("Buffer or Queue space currently has no free space to store data.")]
        BUFFER_FULL = 0x0007,
        [Description("Incoming message exceeds buffer capacity.")]
        PACKET_SIZE_UNSUPPORTED = 0x0008,
        [Description("Sub-Device is out of range or unknown.")]
        SUB_DEVICE_OUT_OF_RANGE = 0x0009,
        [Description("The proxy buffer is full and can not store any more Queued Message or Status Message responses.")]
        PROXY_BUFFER_FULL = 0x000A,
        //E1.37-2
        [Description("The specified action is not supported.")]
        ACTION_NOT_SUPPORTED = 0x000B,
        //E1.33
        [Description("The Component is not participating in the given Scope.")]
        UNKNOWN_SCOPE = 0x000F,
        [Description("The Static Config Type provided is invalid.")]
        INVALID_STATIC_CONFIG_TYPE = 0x0010,
        [Description("The IPv4 Address provided is invalid.")]
        INVALID_IPV4_ADDRESS = 0x0011,
        [Description("The IPv6 Address provided is invalid.")]
        INVALID_IPV6_ADDRESS = 0x0012,
        [Description("The transport layer port provided is invalid.")]
        INVALID_PORT = 0x0013,
        //E1.37-7
        [Description("The transport layer port provided is invalid")]
        ENDPOINT_NUMBER_INVALID = 0x000C,
        [Description("The transport layer port provided is invalid")]
        INVALID_ENDPOINT_MODE = 0x000D,
        [Description("The transport layer port provided is invalid")]
        UNKNOWN_UID = 0x000E
    }
}
