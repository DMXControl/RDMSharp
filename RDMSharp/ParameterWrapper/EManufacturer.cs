using System.ComponentModel;

namespace RDMSharp.ParameterWrapper
{
    public enum EManufacturer : ushort
    {
        [Description("DMXControl-Projects e.V.")]
        DMXControlProjects_eV = 0x02b0,
        [Description("SGM Technology For Lighting SPA")]
        SGM_Technology_For_Lighting_SPA = 5347,
        [Description("Wireless Solution Sweden AB")]
        Wireless_Solution_Sweden_AB = 5753,
        [Description("Steinigke Showtechnic GmbH")]
        Steinigke_Showtechnic_GmbH = 0x29aa,
    }
}