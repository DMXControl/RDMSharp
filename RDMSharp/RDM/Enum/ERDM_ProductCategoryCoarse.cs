namespace RDMSharp
{
    public enum ERDM_ProductCategoryCoarse : byte
    {
        NOT_DECLARED = 0x00,

        FIXTURE = 0x01,
        FIXTURE_ACCESSORY = 0x02,
        PROJECTOR = 0x03,
        ATMOSPHERIC = 0x04,
        DIMMER = 0x05,
        POWER = 0x06,
        SCENIC = 0x07,
        DATA = 0x08,
        AV = 0x09,
        MONITOR = 0x0A,
        CONTROL = 0x70,
        TEST = 0x71,
        OTHER = 0x7f
    }
}
