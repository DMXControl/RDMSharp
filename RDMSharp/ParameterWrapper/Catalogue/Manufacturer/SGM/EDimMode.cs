﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.DIM_MODE, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.DIM_MODE, Command.ECommandDublicte.SetRequest)]
    public enum EDimMode : byte
    {
        STANDARD = 0,
        MAX_POWER = 1
    }
}