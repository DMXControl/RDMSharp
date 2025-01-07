using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.FAN_MODE, Command.ECommandDublicte.GetResponse,"")]
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.FAN_MODE, Command.ECommandDublicte.SetRequest,"")]
    public enum EFanMode : byte
    {
        AUTO = 0,
        LOW = 1,
        HIGH = 2,
        FULL = 3
    }
}