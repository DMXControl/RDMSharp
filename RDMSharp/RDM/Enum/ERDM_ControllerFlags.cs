using System;

namespace RDMSharp;

[Flags]
public enum ERDM_ControllerFlags : byte
{
    None = 0b00000001,
    Unicode = 0b00000001,
    HiResAckTimerSupport = 0b00000010,
}