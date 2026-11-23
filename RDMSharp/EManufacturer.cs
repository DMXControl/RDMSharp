using System.ComponentModel;

namespace RDMSharp;

public enum EManufacturer : ushort
{
    [Description("ESTA")]
    ESTA = 0x0000,
    [Description("GEE")]
    GEE = 0x0001,
    [Description("Wattle IT")]
    Wattle_IT = 0x0020,
    [Description("CHAMP Licht")]
    CHAMP_Licht = 0x0077,
    [Description("Creative Lighting And Sound Systems Pty Ltd.")]
    Creative_Lighting_And_Sound_Systems = 0x00A1,
    [Description("Constell8 NV")]
    Constell8_NV = 0x00C8,
    [Description("Inventronics (Hangzhou), Inc.")]
    Inventronics = 0x0107,
    [Description("Videndum Production Solutions Inc. (Litepanels)")]
    Videndum_Production_Solutions = 0x01AA,
    [Description("Big Bang Lightning (KLSTR)")]
    Big_Bang_Lightning = 0x02A3,
    [Description("DMXControl-Projects e.V.")]
    DMXControlProjects_eV = 0x02b0,
    [Description("CLF Lighting BV.")]
    CLF_Lighting_BV = 0x02E2,
    [Description("ImageCue LLC")]
    ImageCue_LLC = 0x0308,
    [Description("D.T.S. Illuminazione srl")]
    DTS_Illuminazione_srl = 0x0710,
    [Description("QubiCast GmbH")]
    QubiCast_GmbH = 0x082E,
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
    [Description("Steinigke Showtechnic GmbH")]
    Steinigke_Showtechnic_GmbH = 0x29aa,
    [Description("Global Design Solutions, Ltd.")]
    Global_Design_Solutions = 0x3434,
    [Description("ACME EFFECTS LTD.")]
    ACME_Effects_Ltd = 0x3806,
    [Description("Altman Stage Lighting")]
    Altman_Stage_Lighting = 0x4131,
    [Description("Astera")]
    Astera = 0x4173,
    [Description("AYRTON")]
    AYRTON = 0x4179,
    [Description("Coemar Spa")]
    Coemar_Spa = 0x434D,
    [Description("CLAY PAKY S.p.A")]
    CLAY_PAKY_SpA = 0x4350,
    [Description("City Theatrical, Inc.")]
    City_Theatrical = 0x4354,
    [Description("ENTTEC Pty Ltd")]
    ENTTEC = 0x454E,
    [Description("Magic FX B.V.")]
    Magic_FX_bv = 0x4658,
    [Description("Howard Eaton Lighting Ltd.")]
    Howard_Eaton_Lighting = 0x4845,
    [Description("JB-lighting GmbH")]
    JB_lighting_GmbH = 0x4A42,
    [Description("Jands Pty Ltd.")]
    Jands = 0x4A61,
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
    [Description("Philips Entertainment Lighting Asia")]
    Philips_Entertainment_Lighting_Asia = 0x5041,
    [Description("Production Resource Group")]
    Production_Resource_Group = 0x5052,
    [Description("Robe Show Lighting s.r.o.")]
    Robe_Show_Lighting_sro = 0x5253,
    [Description("SGM Technology For Lighting SPA")]
    SGM_Technology_For_Lighting_SPA = 0x5347,
    [Description("Chroma-Q")]
    Chroma_Q = 0x5370,
    [Description("Swisson AG")]
    Swisson_AG = 0x5377,
    [Description("Singularity (UK) Ltd.")]
    Singularity_UK_Ltd = 0x5379,
    [Description("SOUNDLIGHT")]
    SOUNDLIGHT = 0x534C,
    [Description("Wireless Solution Sweden AB")]
    Wireless_Solution_Sweden_AB = 0x5753,
    [Description("ELC lighting")]
    ELC_lighting = 0x656C,
    [Description("Electronic Theatre Controls Inc.")]
    Electronic_Theatre_Controls_Inc = 0x6574,
    [Description("GLP German Light Products GmbH")]
    GLP_German_Light_Products_GmbH = 0x676C,
    [Description("Roxx")]
    Roxx = 0x6a6a,
    [Description("Explorentis (Lucenti)")]
    Explorentis = 0x6FED,
    [Description("Open Lighting")]
    Open_Lighting = 0x7A70,
    [Description("Look Solutions GmbH & Co. KG")]
    Look_Solutions_GmbH_and_Co_KG = 0x7EED
}