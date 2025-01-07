using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    //E1.37-1
    [DataTreeEnum(ERDM_Parameter.IDENTIFY_MODE, Command.ECommandDublicte.GetResponse, "mode")]
    [DataTreeEnum(ERDM_Parameter.IDENTIFY_MODE, Command.ECommandDublicte.SetRequest, "mode")]
    public enum ERDM_IdentifyMode : byte
    {
        QUIET = 0x00,
        LOUD = 0xFF
    }
}
