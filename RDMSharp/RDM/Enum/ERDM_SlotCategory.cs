namespace RDMSharp
{
    public enum ERDM_SlotCategory : ushort
    {
        //Intensity Functions 0x00xx
        INTENSITY = 0x0001,
        INTENSITY_MASTER = 0x0002,

        //Movement Functions 0x01xx
        PAN = 0x0101,
        TILT = 0x0102,

        //Color Functions 0x02xx
        COLOR_WHEEL = 0x0201,
        COLOR_SUB_CYAN = 0x0202,
        COLOR_SUB_YELLOW = 0x0203,
        COLOR_SUB_MAGENTA = 0x0204,
        COLOR_ADD_RED = 0x0205,
        COLOR_ADD_GREEN = 0x0206,
        COLOR_ADD_BLUE = 0x0207,
        COLOR_CORRECTION = 0x0208,
        COLOR_SCROLL = 0x0209,
        COLOR_SEMAPHORE = 0x0210,

        // Image Functions 0x03xx
        STATIC_GOBO_WHEEL = 0x0301,
        ROTO_GOBO_WHEEL = 0x0302,
        PRISM_WHEEL = 0x0303,
        EFFECTS_WHEEL = 0x0304,

        //Beam Functions 0x04xx
        BEAM_SIZE_IRIS = 0x0401,
        EDGE = 0x0402,
        FROST = 0x0403,
        STROBE = 0x0404,
        ZOOM = 0x0405,
        FRAMING_SHUTTER = 0x0406,
        SHUTTER_ROTATE = 0x0407,
        DOUSER = 0x0408,
        BARN_DOOR = 0x0409,

        //Control Functions 0x05xx
        LAMP_CONTROL = 0x0501,
        FIXTURE_CONTROL = 0x0502,
        FIXTURE_SPEED = 0x0503,
        MACRO = 0x0504,

        SD_UNDEFINED = 0xFFFF
    }
}
