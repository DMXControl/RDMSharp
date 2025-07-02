using System.ComponentModel;

namespace RDMSharp
{
    public enum ERDM_SlotCategory : ushort
    {
        //Intensity Functions 0x00xx
        [Description("Intensity")]
        INTENSITY = 0x0001,
        [Description("Intensity Master")]
        INTENSITY_MASTER = 0x0002,

        //Movement Functions 0x01xx
        [Description("Pan")]
        PAN = 0x0101,
        [Description("Tilt")]
        TILT = 0x0102,

        //Color Functions 0x02xx
        [Description("Color Wheel")]
        COLOR_WHEEL = 0x0201,
        [Description("Subtractive Color Mixer – Cyan/Blue")]
        COLOR_SUB_CYAN = 0x0202,
        [Description("Subtractive Color Mixer – Yellow/Amber")]
        COLOR_SUB_YELLOW = 0x0203,
        [Description("Subtractive Color Mixer - Magenta")]
        COLOR_SUB_MAGENTA = 0x0204,
        [Description("Additive Color Mixer - Red")]
        COLOR_ADD_RED = 0x0205,
        [Description("Additive Color Mixer - Green")]
        COLOR_ADD_GREEN = 0x0206,
        [Description("Additive Color Mixer - Blue")]
        COLOR_ADD_BLUE = 0x0207,
        [Description("Color Temperature Correction")]
        COLOR_CORRECTION = 0x0208,
        [Description("Color Scroll")]
        COLOR_SCROLL = 0x0209,
        [Description("Color Semaphore")]
        COLOR_SEMAPHORE = 0x0210,
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [Description("Additive Color Mixer - Amber")]
        COLOR_ADD_AMBER = 0x0211,
        [Description("Additive Color Mixer - White")]
        COLOR_ADD_WHITE = 0x0212,
        [Description("Additive Color Mixer - Warm White")]
        COLOR_ADD_WARM_WHITE = 0x0213,
        [Description("Additive Color Mixer - Cool White")]
        COLOR_ADD_COOL_WHITE = 0x0214,
        [Description("Subtractive Color Mixer - UV")]
        COLOR_SUB_UV = 0x0215,
        [Description("Hue")]
        COLOR_HUE = 0x0216,
        [Description("Saturation")]
        COLOR_SATURATION = 0x0217,
        [Description("Additive Color Mixer - UV")]
        COLOR_ADD_UV = 0x0218,
        [Description("CIE X Color Coordinate")] //Missing in E1.20-2025
        CIE_X = 0x0219,
        [Description("CIE Y Color Coordinate")] //Missing in E1.20-2025
        CIE_Y = 0x021A,
        [Description("CCT Magenta-Green Adjustment")] //Missing in E1.20-2025
        MAGENTA_GREEN_CORRECTION = 0x021B,
        #endregion

        // Image Functions 0x03xx
        [Description("Static gobo wheel")]
        STATIC_GOBO_WHEEL = 0x0301,
        [Description("Rotating gobo wheel")]
        ROTO_GOBO_WHEEL = 0x0302,
        [Description("Prism wheel")]
        PRISM_WHEEL = 0x0303,
        [Description("Effects wheel")]
        EFFECTS_WHEEL = 0x0304,

        //Beam Functions 0x04xx
        [Description("Beam size iris")]
        BEAM_SIZE_IRIS = 0x0401,
        [Description("Edge/Lens focus")]
        EDGE = 0x0402,
        [Description("Frost/Diffusion")]
        FROST = 0x0403,
        [Description("Strobe/Shutter")]
        STROBE = 0x0404,
        [Description("Zoom lens")]
        ZOOM = 0x0405,
        [Description("Framing shutter")]
        FRAMING_SHUTTER = 0x0406,
        [Description("Framing shutter rotation")]
        SHUTTER_ROTATE = 0x0407,
        [Description("Douser")]
        DOUSER = 0x0408,
        [Description("Barn Door")]
        BARN_DOOR = 0x0409,

        //Control Functions 0x05xx
        [Description("Lamp control functions")]
        LAMP_CONTROL = 0x0501,
        [Description("Fixture control channel")]
        FIXTURE_CONTROL = 0x0502,
        [Description("Overall speed setting applied to multiple or all parameters")]
        FIXTURE_SPEED = 0x0503,
        [Description("Macro control")]
        MACRO = 0x0504,
        # region https://tsp.esta.org/tsp/working_groups/CP/RDMextras.html Additions
        [Description("Relay or Power Control")]
        POWER_CONTROL = 0x0505,
        [Description("Fan Control")]
        FAN_CONTROL = 0x0506,
        [Description("Heater Control")]
        HEATER_CONTROL = 0x0507,
        [Description("Fountain Water Pump Control")]
        FOUNTAIN_CONTROL = 0x0508,
        #endregion

        [Description("No definition")]
        SD_UNDEFINED = 0xFFFF
    }
}
