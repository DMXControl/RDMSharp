using System;

namespace RDMSharp
{
    [Flags]
    public enum ERDM_CommandClass : byte
    {
        NONE = 0x00,
        GET = 0x01,
        SET = 0x02,
        //GET_SET = 0x03// not needet, use Flags pgrote 07.12.2021
    }
}
