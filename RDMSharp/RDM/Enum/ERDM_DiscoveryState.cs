namespace RDMSharp
{
    //E1.37-7
    public enum ERDM_DiscoveryState : byte
    {
        INCOMPLETE = 0x00,
        INCREMENTAL = 0x01,
        FULL = 0x02,
        NOT_ACTIVE = 0x04
        //0x80 - 0xDF Manufacturer Specific
    }
}
