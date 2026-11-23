using System.ComponentModel;

namespace RDMSharp;

public enum EManufacturer : ushort
{
    [Description("ESTA")]
    ESTA = 0x0000,
    [Description("DMXControl-Projects e.V.")]
    DMXControlProjects_eV = 0x02b0,
    [Description("CLF Lighting BV.")]
    CLF_Lighting_BV = 0x02E2,
    [Description("D.T.S. Illuminazione srl")]
    DTS_Illuminazione_srl = 0x0710,
    [Description("Adam Hall GmbH")]
    Adam_Hall = 0x08A4,
    [Description("LMP Lichttechnik Vertriebsgesellschaft GmbH & Co KG")]
    LMP_Lichttechnik_Vertriebsgesellschaft = 0x08E4,
    [Description("Music & Lights S.r.l.")]
    Music_and_Lights_Srl = 0x15D0,
    [Description("HBZZ (not registered at ESTA)")]
    HBZZ = 0x1DD6,
    [Description("ARRI")]
    ARRI = 0x20B9,
    [Description("CHAUVET Lighting")]
    CHAUVET_Lighting = 0x21A4,
    [Description("Elation Lighting Inc.")]
    Elation_Lighting_Inc = 0x22A6,
    [Description("Litecraft (not registered at ESTA)")]
    Litecraft = 0x2353,
    [Description("ACME EFFECTS LTD.")]
    ACME_Effects_Ltd = 0x3806,
    [Description("AYRTON")]
    AYRTON = 0x4179,
    [Description("Coemar Spa")]
    Coemar_Spa = 0x434D,
    [Description("CLAY PAKY S.p.A")]
    CLAY_PAKY_SpA = 0x4350,
    [Description("JB-lighting GmbH")]
    JB_lighting_GmbH = 0x4A42,
    [Description("Philips Entertainment Lighting Asia")]
    Philips_Entertainment_Lighting_Asia = 0x5041,
    [Description("Chroma-Q")]
    Chroma_Q = 0x5370,
    [Description("ELC lighting")]
    ELC_lighting = 0x656C,
    [Description("Look Solutions GmbH & Co. KG")]
    Look_Solutions_GmbH_and_Co_KG = 0x7EED,
    [Description("Astera")]
    Astera = 0x4173,
    [Description("Magic FX B.V.")]
    Magic_FX_bv = 0x4658,
    [Description("Luminex")]
    LUMINEX_Lighting_Control_Equipment_bvba = 0x4C4C,
    [Description("LumenRadio AB")]
    LumenRadio_AB = 0x4C55,
    [Description("LSC Control Systems Pty Ltd")]
    LSC = 0x4C73,
    [Description("MDG")]
    MDG = 0x4D44,
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
    [Description("Electronic Theatre Controls Inc.")]
    Electronic_Theatre_Controls_Inc = 0x6574,
    [Description("Roxx")]
    Roxx = 0x6a6a,
    [Description("Steinigke Showtechnic GmbH")]
    Steinigke_Showtechnic_GmbH = 0x29aa
}