using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    //E1.37-1
    [DataTreeObject(ERDM_Parameter.IDENTIFY_MODE, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(ERDM_Parameter.IDENTIFY_MODE, Command.ECommandDublicte.SetRequest)]
    public enum ERDM_IdentifyMode : byte
    {
        QUIET = 0x00,
        LOUD = 0xFF
    }
}
