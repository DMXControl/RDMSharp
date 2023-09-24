﻿namespace RDMSharp
{
    public enum ERDM_SensorType : byte
    {
        TEMPERATURE = 0x00,
        VOLTAGE = 0x01,
        CURRENT = 0x02,
        FREQUENCY = 0x03,
        RESISTANCE = 0x04,
        POWER = 0x05,
        MASS = 0x06,
        LENGTH = 0x07,
        AREA = 0x08,
        VOLUME = 0x09,
        DENSITY = 0x0A,
        VELOCITY = 0x0B,
        ACCELERATION = 0x0C,
        FORCE = 0x0D,
        ENERGY = 0x0E,
        PRESSURE = 0x0F,
        TIME = 0x10,
        ANGLE = 0x11,
        POSITION_X = 0x12,
        POSITION_Y = 0x13,
        POSITION_Z = 0x14,
        ANGULAR_VELOCITY = 0x15,
        LUMINOUS_INTENSITY = 0x16,
        LUMINOUS_FLUX = 0x17,
        ILLUMINANCE = 0x18,
        CHROMINANCE_RED = 0x19,
        CHROMINANCE_GREEN = 0x1A,
        CHROMINANCE_BLUE = 0x1B,
        CONTACTS = 0x1C,
        MEMORY = 0x1D,
        ITEMS = 0x1E,
        HUMIDITY = 0x1F,
        COUNTER_16BIT = 0x20,
        OTHER = 0x7F
    }
}