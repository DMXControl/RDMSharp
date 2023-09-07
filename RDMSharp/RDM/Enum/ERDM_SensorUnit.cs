namespace RDMSharp
{
    public enum ERDM_SensorUnit : byte
    {
        NONE = 0x00,
        CENTIGRADE = 0x01,
        VOLTS_DC = 0x02,
        VOLTS_AC_PEAK = 0x03,
        VOLTS_AC_RMS = 0x04,
        AMPERE_DC = 0x05,
        AMPERE_AC_PEAK = 0x06,
        AMPERE_AC_RMS = 0x07,
        HERTZ = 0x08,
        OHM = 0x09,
        WATT = 0x0A,
        KILOGRAM = 0x0B,
        METERS = 0x0C,
        METERS_SQUARED = 0x0D,
        METERS_CUBED = 0x0E,
        KILOGRAMMES_PER_METER_CUBED = 0x0F,
        METERS_PER_SECOND = 0x10,
        METERS_PER_SECOND_SQUARED = 0x11,
        NEWTON = 0x12,
        JOULE = 0x13,
        PASCAL = 0x14,
        SECOND = 0x15,
        DEGREE = 0x16,
        STERADIAN = 0x17,
        CANDELA = 0x18,
        LUMEN = 0x19,
        LUX = 0x1A,
        IRE = 0x1B,
        BYTE = 0x1C
    }
}
