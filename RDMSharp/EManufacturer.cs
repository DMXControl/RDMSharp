using System.ComponentModel;

namespace RDMSharp;

public enum EManufacturer : ushort
{
    [Description("ESTA")]
    ESTA = 0x0000,
    [Description("DMXControl-Projects e.V.")]
    DMXControlProjects_eV = 0x02b0,
    [Description("Magic FX B.V.")]
    Magic_FX_bv = 0x4658,
    [Description("Luminex")]
    LUMINEX_Lighting_Control_Equipment_bvba = 0x4C4C,
    [Description("LumenRadio AB")]
    LumenRadio_AB = 0x4C55,
    [Description("Martin Professional A/S")]
    Martin_Professional_AS = 0x4D50,
    [Description("SGM Technology For Lighting SPA")]
    SGM_Technology_For_Lighting_SPA = 0x5347,
    [Description("Wireless Solution Sweden AB")]
    Wireless_Solution_Sweden_AB = 0x5753,
    [Description("Robe Show Lighting s.r.o.")]
    Robe_Show_Lighting_sro = 0x5253,
    [Description("Swisson AG")]
    Swisson_AG = 0x5377,
    [Description("GLP German Light Products GmbH")]
    GLP_German_Light_Products_GmbH = 0x676C,
    [Description("Steinigke Showtechnic GmbH")]
    Steinigke_Showtechnic_GmbH = 0x29aa,
}