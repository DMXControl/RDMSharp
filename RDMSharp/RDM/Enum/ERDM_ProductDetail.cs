namespace RDMSharp
{
    public enum ERDM_ProductDetail : ushort
    {
        NONE = 0x0000,

        //Generally applied to fixtures
        ARC = 0x0001,
        METAL_HALIDE = 0x0002,
        INCANDESCENT = 0x0003,
        LED = 0x0004,
        FLUROESCENT = 0x0005,
        COLDCATHODE = 0x0006,
        ELECTROLUMINESCENT = 0x0007,
        LASER = 0x0008,
        FLASHTUBE = 0x0009,

        //Generally applied to fixture accessories
        COLORSCROLLER = 0x0100,
        COLORWHEEL = 0x0101,
        COLORCHANGE = 0x0102,
        IRIS_DOUSER = 0x0103,
        DIMMING_SHUTTER = 0x0104,
        PROFILE_SHUTTER = 0x0105,
        BARNDOOR_SHUTTER = 0x0106,
        EFFECTS_DISC = 0x0107,
        GOBO_ROTATOR = 0x0108,

        //Generally applied to Projectors
        VIDEO = 0x0200,
        SLIDE = 0x0201,
        FILM = 0x0202,
        OILWHEEL = 0x0203,
        LCDGATE = 0x0204,

        //Generally applied to Atmospheric Effects
        FOGGER_GLYCOL = 0x0300,
        FOGGER_MINERALOIL = 0x0301,
        FOGGER_WATER = 0x0302,
        CO2 = 0x0303,
        LN2 = 0x0304,
        BUBBLE = 0x0305,
        FLAME_PROPANE = 0x0306,
        FLAME_OTHER = 0x0307,
        OLEFACTORY_STIMULATOR = 0x0308,
        SNOW = 0x0309,
        WATER_JET = 0x030A,
        WIND = 0x030B,
        CONFETTI = 0x030C,
        HAZARD = 0x030D,

        //Generally applied to Dimmers/Power controllers
        PHASE_CONTROL = 0x0400,
        REVERSE_PHASE_CONTROL = 0x0401,
        SINE = 0x0402,
        PWM = 0x0403,
        DC = 0x0404,
        HFBALLAST = 0x0405,
        HFHV_NEONBALLAST = 0x0406,
        HFHV_EL = 0x0407,
        MHR_BALLAST = 0x0408,
        BITANGLE_MODULATION = 0x0409,
        FREQUENCY_MODULATION = 0x040A,
        HIGHFREQUENCY_12V = 0x040B,
        RELAY_MECHANICAL = 0x040C,
        RELAY_ELECTRONIC = 0x040D,
        SWITCH_ELECTRONIC = 0x040E,
        CONTACTOR = 0x040F,

        //Generally applied to Scenic drive
        MIRRORBALL_ROTATOR = 0x0500,
        OTHER_ROTATOR = 0x0501,
        KABUKI_DROP = 0x0502,
        CURTAIN = 0x0503,
        LINESET = 0x0504,
        MOTOR_CONTROL = 0x0505,
        DAMPER_CONTROL = 0x0506,

        //Generally applied to Data Distribution
        SPLITTER = 0x0600,
        ETHERNET_NODE = 0x0601,
        MERGE = 0x0602,
        DATAPATCH = 0x0603,
        WIRELESS_LINK = 0x0604,

        //Generally applied to Data Conversion and Interfaces
        PROTOCOL_CONVERTOR = 0x0701,
        ANALOG_DEMULTIPLEX = 0x0702,
        ANALOG_MULTIPLEX = 0x0703,
        SWITCH_PANEL = 0x0704,

        //Generally applied to Audio or Video (AV) devices
        ROUTER = 0x0800,
        FADER = 0x0801,
        MIXER = 0x0802,

        //Generally applied to Controllers, Backup devices and Test Equipment
        CHANGEOVER_MANUAL = 0x0900,
        CHANGEOVER_AUTO = 0x0901,
        TEST = 0x0902,

        //Could be applied to any category
        GFI_RCD = 0x0A00,
        BATTERY = 0x0A01,
        CONTROLLABLE_BREAKER = 0x0A02,

        //Manufacturer Specific Types 0x8000 - 0xDFFF


        OTHER = 0x7FFF,
    }
}
