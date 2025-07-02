namespace RDMSharp
{
    public enum ERDM_ResponseType : byte
    {
        ACK = 0x00,
        ACK_TIMER = 0x01,
        NACK_REASON = 0x02,
        ACK_OVERFLOW = 0x03,
        ACK_TIMER_HI_RES = 0x04
    }
}
