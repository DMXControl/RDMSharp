using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;

namespace RDMSharp
{
    [DataTreeEnum(ERDM_Parameter.STATUS_MESSAGES, Command.ECommandDublicate.GetRequest, "status_type")]
    [DataTreeEnum(ERDM_Parameter.QUEUED_MESSAGE, Command.ECommandDublicate.GetRequest, "status_type")]
    [DataTreeEnum(ERDM_Parameter.SUB_DEVICE_STATUS_REPORT_THRESHOLD, Command.ECommandDublicate.GetResponse, "status_type")]
    [DataTreeEnum(ERDM_Parameter.SUB_DEVICE_STATUS_REPORT_THRESHOLD, Command.ECommandDublicate.SetRequest, "status_type")]

    [Flags]
    public enum ERDM_Status : byte
    {
        NONE = 0x00,

        GET_LAST_MESSAGE = 0x01,

        ADVISORY = 0x02,
        WARNING = 0x03,
        ERROR = 0x04,

        CLEARED = 0x10,

        ADVISORY_CLEARED = 0x12,
        WARNING_CLEARED = 0x13,
        ERROR_CLEARED = 0x14,
    }
}
