using System;

namespace RDMSharp
{
    [Flags]
    public enum ERDM_SupportedSubDevice : byte
    {
        NONE = 0x00,
        ROOT = 0x01,
        RANGE_0X0001_0x0200 = 0x02,
        BROADCAST = 0x08,
        ALL_EXCEPT_ROOT = RANGE_0X0001_0x0200 | BROADCAST,
        ALL_EXCEPT_BROADCAST = ROOT | RANGE_0X0001_0x0200,
        ALL = ROOT | RANGE_0X0001_0x0200 | BROADCAST
    }
}
