namespace RDMSharp
{
    //E1.37-5
    public enum ERDM_ShippingLockState : byte
    {
        /// <summary>
        /// All axes that are capable of being mechanically locked are free.
        /// </summary>
        UNLOCKED = 0x00,
        /// <summary>
        /// All axes that are capable of being mechanically locked are immobile.
        /// </summary>
        LOCKED = 0x01,
        /// <summary>
        /// Some axes are locked, restricted movement allowed.
        /// </summary>
        PARTIALLY_LOCKED = 0x02,
    }
}
