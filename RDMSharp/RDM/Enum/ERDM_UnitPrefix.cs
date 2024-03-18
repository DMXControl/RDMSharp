using System;

namespace RDMSharp
{
    public enum ERDM_UnitPrefix : byte
    {
        [UnitPrefixAttribute(1)]
        NONE = 0x00,
        [UnitPrefixAttribute(1E-1)]
        DECI = 0x01,
        [UnitPrefixAttribute(1E-2)]
        CENTI = 0x02,
        [UnitPrefixAttribute(1E-3)]
        MILLI = 0x03,
        [UnitPrefixAttribute(1E-6)]
        MICRO = 0x04,
        [UnitPrefixAttribute(1E-9)]
        NANO = 0x05,
        [UnitPrefixAttribute(1E-12)]
        PICO = 0x06,
        [UnitPrefixAttribute(1E-15)]
        FEMPTO = 0x07,
        [UnitPrefixAttribute(1E-18)]
        ATTO = 0x08,
        [UnitPrefixAttribute(1E-21)]
        ZEPTO = 0x09,
        [UnitPrefixAttribute(1E-24)]
        YOCTO = 0x0A,
        [UnitPrefixAttribute(1E1)]
        DECA = 0x11,
        [UnitPrefixAttribute(1E2)]
        HECTO = 0x12,
        [UnitPrefixAttribute(1E3)]
        KILO = 0x13,
        [UnitPrefixAttribute(1E6)]
        MEGA = 0x14,
        [UnitPrefixAttribute(1E9)]
        GIGA = 0x15,
        [UnitPrefixAttribute(1E12)]
        TERRA = 0x16,
        [UnitPrefixAttribute(1E15)]
        PETA = 0x17,
        [UnitPrefixAttribute(1E18)]
        EXA = 0x18,
        [UnitPrefixAttribute(1E21)]
        ZETTA = 0x19,
        [UnitPrefixAttribute(1E24)]
        YOTTA = 0x1A
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class UnitPrefixAttribute : Attribute
    {
        public readonly double Multiplyer;
        public UnitPrefixAttribute(double multiplyer)
        {
            Multiplyer = multiplyer;
        }
    }
}
