using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.RESET_DEVICE, Command.ECommandDublicte.SetRequest)]
    public enum ERDM_ResetType : byte
    {
        Warm = 0x01,
        Cold = 0xFF
    }
}
