using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeEnum(ERDM_Parameter.DISPLAY_INVERT, Command.ECommandDublicte.GetResponse, "setting")]
    [DataTreeEnum(ERDM_Parameter.DISPLAY_INVERT, Command.ECommandDublicte.SetRequest, "setting")]
    public enum ERDM_DisplayInvert : byte
    {
        OFF = 0x00,
        ON = 0x01,
        AUTO = 0x02,
    }
}
