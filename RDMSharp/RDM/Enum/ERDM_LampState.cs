using System.ComponentModel;

namespace RDMSharp
{
    public enum ERDM_LampState : byte
    {
        [Description("Off")]
        OFF = 0x00,
        [Description("On")]
        ON = 0x01,
        [Description("Arc-Lamp ignite (Strike)")]
        STRIKE = 0x02,
        [Description("Arc-Lamp Reduced Power Mode (Standby)")]
        STANDBY = 0x03,
        [Description("Lamp not installed")]
        NOT_PRESENT = 0x04,
        [Description("Error")]
        ERROR = 0x7F,
        //0x80 - 0xDF Manufacturer Specific
    }
}
