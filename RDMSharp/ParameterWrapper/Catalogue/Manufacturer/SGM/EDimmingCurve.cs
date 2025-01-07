using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.DIMMING_CURVE, Command.ECommandDublicte.GetResponse,"")]
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.DIMMING_CURVE, Command.ECommandDublicte.SetRequest,"")]
    public enum EDimmingCurve : byte
    {
        RAW = 0,
        GAMMA_CORRECTED = 1
    }
}