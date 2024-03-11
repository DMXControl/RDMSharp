using System.ComponentModel;

namespace RDMSharp
{
    public enum ERDM_SlotType : byte
    {
        [Description("Slot directly controls parameter (represents Coarse for 16-bit parameters)")]
        PRIMARY = 0x00,

        [Description("Fine, for 16-bit parameters")]
        SEC_FINE = 0x01,
        [Description("Slot sets timing value for associated parameter")]
        SEC_TIMING = 0x02,
        [Description("Slot sets speed/velocity for associated parameter")]
        SEC_SPEED = 0x03,
        [Description("Slot provides control/mode info for parameter")]
        SEC_CONTROL = 0x04,
        [Description("Slot sets index position for associated parameter")]
        SEC_INDEX = 0x05,
        [Description("Slot sets rotation speed for associated parameter")]
        SEC_ROTATION = 0x06,
        [Description("Combined index/rotation control")]
        SEC_INDEX_ROTATE = 0x07,

        [Description("Undefined secondary type")]
        SEC_UNDEFINED = 0xFF
    }
}
