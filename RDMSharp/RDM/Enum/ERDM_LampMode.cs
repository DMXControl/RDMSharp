using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System.ComponentModel;

namespace RDMSharp;

[DataTreeObject(ERDM_Parameter.LAMP_ON_MODE, Command.ECommandDublicte.GetResponse)]
[DataTreeObject(ERDM_Parameter.LAMP_ON_MODE, Command.ECommandDublicte.SetRequest)]
public enum ERDM_LampMode : byte
{
    [Description("Lamp Stays off until directly instructed to Strike.")]
    ON_MODE_OFF = 0x00,
    [Description("Lamp Strikes upon receiving a DMX512 signal.")]
    ON_MODE_DMX = 0x01,
    [Description("Lamp Strikes automatically at Power-up.")]
    ON_MODE_ON = 0x02,
    [Description("Lamp Strikes after Calibration or Homing procedure.")]
    ON_MODE_AFTER_CAL = 0x03,
}