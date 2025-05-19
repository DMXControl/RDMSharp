using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeEnum(ERDM_Parameter.POWER_STATE, Command.ECommandDublicte.GetResponse, "state")]
    [DataTreeEnum(ERDM_Parameter.POWER_STATE, Command.ECommandDublicte.SetRequest, "state")]
    public enum ERDM_PowerState : byte
    {
        FULL_OFF = 0x00,
        SHUTDOWN = 0x01,
        STANDBY = 0x02,
        NORMAL = 0xFF
    }
}
