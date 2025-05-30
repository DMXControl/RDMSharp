using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeEnum(ERDM_Parameter.RESET_DEVICE, Command.ECommandDublicate.SetRequest, "state")]
    public enum ERDM_ResetType : byte
    {
        Warm = 0x01,
        Cold = 0xFF
    }
}
