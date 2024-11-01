using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.DISPLAY_INVERT, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(ERDM_Parameter.DISPLAY_INVERT, Command.ECommandDublicte.SetRequest)]
    public enum ERDM_DisplayInvert : byte
    {
        OFF = 0x00,
        ON = 0x01,
        AUTO = 0x02,
    }
}
