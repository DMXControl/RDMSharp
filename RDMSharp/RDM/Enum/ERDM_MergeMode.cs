﻿using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp
{
    [DataTreeEnum(ERDM_Parameter.PRESET_MERGEMODE, Command.ECommandDublicte.GetResponse, "mode")]
    [DataTreeEnum(ERDM_Parameter.PRESET_MERGEMODE, Command.ECommandDublicte.SetRequest, "mode")]
    public enum ERDM_MergeMode : byte
    {
        DEFAULT = 0x00,
        HTP = 0x01,
        LTP = 0x02,
        DMX_ONLY = 0x03,
        OTHER = 0xFF
    }
}
