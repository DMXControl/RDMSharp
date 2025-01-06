using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;

namespace RDMSharp.ParameterWrapper.SGM
{
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.BATTERY_EXTENSION, Command.ECommandDublicte.GetResponse)]
    [DataTreeObject(EManufacturer.SGM_Technology_For_Lighting_SPA, (ERDM_Parameter)(ushort)EParameter.BATTERY_EXTENSION, Command.ECommandDublicte.SetRequest)]
    public enum EBatteryExtension : byte
    {
        DISABLED = 0,
        _1H = 1,
        _2H = 2,
        _3H = 3,
        _4H = 4,
        _5H = 5,
        _6H = 6,
        _7H = 7,
        _8H = 8,
        _9H = 9,
        _10H = 10,
        _11H = 11,
        _12H = 12,
        _13H = 13,
        _14H = 14,
        _15H = 15,
        _16H = 16,
        _17H = 17,
        _18H = 18,
        _19H = 19,
        _20H = 20,
        _21H = 21,
        _22H = 22,
        _23H = 23,
        _24H = 24
    }
}