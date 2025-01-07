﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.INVERT_PIXEL_ORDER, Command.ECommandDublicte.GetResponse,"")]
    [DataTreeEnum(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.INVERT_PIXEL_ORDER, Command.ECommandDublicte.SetRequest,"")]
    public enum EInvertPixelOrder : byte
    {
        STANDARD = 0,
        INVERT = 1
    }
}