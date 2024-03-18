using System;

namespace RDMSharp
{
    public enum ERDM_SensorUnit : byte
    {
        [SensorUnitAttribute("")]
        NONE = 0x00,
        [SensorUnitAttribute("°C")]
        CENTIGRADE = 0x01,
        [SensorUnitAttribute("V")]
        VOLTS_DC = 0x02,
        [SensorUnitAttribute("Vₚ")]
        VOLTS_AC_PEAK = 0x03,
        [SensorUnitAttribute("Vᵣₘₛ")]
        VOLTS_AC_RMS = 0x04,
        [SensorUnitAttribute("A")]
        AMPERE_DC = 0x05,
        [SensorUnitAttribute("Aₚ")]
        AMPERE_AC_PEAK = 0x06,
        [SensorUnitAttribute("Aᵣₘₛ")]
        AMPERE_AC_RMS = 0x07,
        [SensorUnitAttribute("Hz")]
        HERTZ = 0x08,
        [SensorUnitAttribute("Ω")]
        OHM = 0x09,
        [SensorUnitAttribute("W")]
        WATT = 0x0A,
        [SensorUnitAttribute("kg")]
        KILOGRAM = 0x0B,
        [SensorUnitAttribute("m")]
        METERS = 0x0C,
        [SensorUnitAttribute("m²")]
        METERS_SQUARED = 0x0D,
        [SensorUnitAttribute("m³")]
        METERS_CUBED = 0x0E,
        [SensorUnitAttribute("kg/m³")]
        KILOGRAMMES_PER_METER_CUBED = 0x0F,
        [SensorUnitAttribute("m/s")]
        METERS_PER_SECOND = 0x10,
        [SensorUnitAttribute("m/s²")]
        METERS_PER_SECOND_SQUARED = 0x11,
        [SensorUnitAttribute("N")]
        NEWTON = 0x12,
        [SensorUnitAttribute("J")]
        JOULE = 0x13,
        [SensorUnitAttribute("Pa")]
        PASCAL = 0x14,
        [SensorUnitAttribute("s")]
        SECOND = 0x15,
        [SensorUnitAttribute("°")]
        DEGREE = 0x16,
        [SensorUnitAttribute("sr")]
        STERADIAN = 0x17,
        [SensorUnitAttribute("cd")]
        CANDELA = 0x18,
        [SensorUnitAttribute("lm")]
        LUMEN = 0x19,
        [SensorUnitAttribute("lx")]
        LUX = 0x1A,
        [SensorUnitAttribute("IRE")]
        IRE = 0x1B,
        [SensorUnitAttribute("B")]
        BYTE = 0x1C
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SensorUnitAttribute : Attribute
    {
        public readonly string Unit;
        public SensorUnitAttribute(string unit)
        {
            Unit = unit;
        }
    }
}
