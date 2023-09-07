namespace RDMSharp
{
    public enum ERDM_ProductCategoryFine : ushort
    {
        NOT_DECLARED = 0x0000,

        /* Fixtures - intended as source of illumination See Note 1                                                                     */
        FIXTURE = 0x0100, /* No Fine Category declared                                   */
        FIXTURE_FIXED = 0x0101, /* No pan / tilt / focus style functions                       */
        FIXTURE_MOVING_YOKE = 0x0102,
        FIXTURE_MOVING_MIRROR = 0x0103,
        FIXTURE_OTHER = 0x01FF, /* For example, focus but no pan/tilt.                         */

        /* Fixture Accessories - add-ons to fixtures or projectors                                                                      */
        FIXTURE_ACCESSORY = 0x0200, /* No Fine Category declared.                                  */
        FIXTURE_ACCESSORY_COLOR = 0x0201, /* Scrollers / Color Changers                                  */
        FIXTURE_ACCESSORY_YOKE = 0x0202, /* Yoke add-on                                                 */
        FIXTURE_ACCESSORY_MIRROR = 0x0203, /* Moving mirror add-on                                        */
        FIXTURE_ACCESSORY_EFFECT = 0x0204, /* Effects Discs                                               */
        FIXTURE_ACCESSORY_BEAM = 0x0205, /* Gobo Rotators /Iris / Shutters / Dousers/ Beam modifiers.   */
        FIXTURE_ACCESSORY_OTHER = 0x02FF,

        /* Projectors - light source capable of producing realistic images from another media i.e Video / Slide / Oil Wheel / Film */
        PROJECTOR = 0x0300, /* No Fine Category declared.                                  */
        PROJECTOR_FIXED = 0x0301, /* No pan / tilt functions.                                    */
        PROJECTOR_MOVING_YOKE = 0x0302,
        PROJECTOR_MOVING_MIRROR = 0x0303,
        PROJECTOR_OTHER = 0x03FF,

        /* Atmospheric Effect - earth/wind/fire                                                                                         */
        ATMOSPHERIC = 0x0400, /* No Fine Category declared.                                  */
        ATMOSPHERIC_EFFECT = 0x0401, /* Fogger / Hazer / Flame, etc.                                */
        ATMOSPHERIC_PYRO = 0x0402, /* See Note 2.                                                 */
        ATMOSPHERIC_OTHER = 0x04FF,

        /* Intensity Control (specifically Dimming equipment)                                                                           */
        DIMMER = 0x0500, /* No Fine Category declared.                                  */
        DIMMER_AC_INCANDESCENT = 0x0501, /* AC > 50VAC                                                  */
        DIMMER_AC_FLUORESCENT = 0x0502,
        DIMMER_AC_COLDCATHODE = 0x0503, /* High Voltage outputs such as Neon or other cold cathode.    */
        DIMMER_AC_NONDIM = 0x0504, /* Non-Dim module in dimmer rack.                              */
        DIMMER_AC_ELV = 0x0505, /* AC <= 50V such as 12/24V AC Low voltage lamps.              */
        DIMMER_AC_OTHER = 0x0506,
        DIMMER_DC_LEVEL = 0x0507, /* Variable DC level output.                                   */
        DIMMER_DC_PWM = 0x0508, /* Chopped (PWM) output.                                       */
        DIMMER_CS_LED = 0x0509, /* Specialized LED dimmer.                                     */
        DIMMER_OTHER = 0x05FF,

        /* Power Control (other than Dimming equipment)                                                                                 */
        POWER = 0x0600, /* No Fine Category declared.                                  */
        POWER_CONTROL = 0x0601, /* Contactor racks, other forms of Power Controllers.          */
        POWER_SOURCE = 0x0602, /* Generators                                                  */
        POWER_OTHER = 0x06FF,

        /* Scenic Drive - including motorized effects unrelated to light source.                                                        */
        SCENIC = 0x0700, /* No Fine Category declared                                   */
        SCENIC_DRIVE = 0x0701, /* Rotators / Kabuki drops, etc. See Note 2.                   */
        SCENIC_OTHER = 0x07FF,

        /* DMX Infrastructure, conversion and interfaces                                                                                */
        DATA = 0x0800, /* No Fine Category declared.                                  */
        DATA_DISTRIBUTION = 0x0801, /* Splitters/repeaters/Ethernet products used to distribute DMX*/
        DATA_CONVERSION = 0x0802, /* Protocol Conversion analog decoders.                        */
        DATA_OTHER = 0x08FF,

        /* Audio-Visual Equipment                                                                                                       */
        AV = 0x0900, /* No Fine Category declared.                                  */
        AV_AUDIO = 0x0901, /* Audio controller or device.                                 */
        AV_VIDEO = 0x0902, /* Video controller or display device.                         */
        AV_OTHER = 0x09FF,

        /* Parameter Monitoring Equipment See Note 3.                                                                                   */
        MONITOR = 0x0A00, /* No Fine Category declared.                                  */
        MONITOR_ACLINEPOWER = 0x0A01, /* Product that monitors AC line voltage, current or power.    */
        MONITOR_DCPOWER = 0x0A02, /* Product that monitors DC line voltage, current or power.    */
        MONITOR_ENVIRONMENTAL = 0x0A03, /* Temperature or other environmental parameter.               */
        MONITOR_OTHER = 0x0AFF,

        /* Controllers, Backup devices                                                                                                  */
        CONTROL = 0x7000, /* No Fine Category declared.                                  */
        CONTROL_CONTROLLER = 0x7001,
        CONTROL_BACKUPDEVICE = 0x7002,
        CONTROL_OTHER = 0x70FF,

        /* Test Equipment                                                                                                               */
        TEST = 0x7100, /* No Fine Category declared.                                  */
        TEST_EQUIPMENT = 0x7101,
        TEST_EQUIPMENT_OTHER = 0x71FF,

        /* Miscellaneous                                                                                                                */
        OTHER = 0x7FFF /* For devices that aren't described within this table.        */
    }
}
