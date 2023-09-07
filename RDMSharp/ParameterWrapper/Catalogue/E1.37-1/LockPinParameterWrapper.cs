namespace RDMSharp.ParameterWrapper
{
    public sealed class LockPinParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ushort, SetLockPinRequest>
    {
        public LockPinParameterWrapper() : base(ERDM_Parameter.LOCK_PIN)
        {
        }
        public override string Name => "Lock PIN";
        public override string Description => 
            "This parameter is used to get and set the PIN code for devices that support locking. The lock state is set using the Lock State message.\r\n\r\n" +
            "The PIN format depends on the capabilities of the device.";

        protected override ushort getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ushort value)
        {
            return Tools.ValueToData(value);
        }

        protected override SetLockPinRequest setRequestParameterDataToValue(byte[] parameterData)
        {
            return SetLockPinRequest.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(SetLockPinRequest setLockPin)
        {
            return setLockPin.ToPayloadData();
        }
    }
}