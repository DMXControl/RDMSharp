using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.BROKER_STATUS, Command.ECommandDublicte.SetRequest)]
    public enum ERDM_BrokerStatus : byte
    {
        DISABLED = 0x00,
        ACTIVE = 0x01,
        STANDBY = 0x02
    }
}
