namespace RDMSharp.ParameterWrapper
{
    public sealed class ShippingLockParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_ShippingLockState, ERDM_ShippingLockState>
    {
        public ShippingLockParameterWrapper() : base(ERDM_Parameter.SHIPPING_LOCK)
        {
        }
        public override string Name => "Shipping Lock";
        public override string Description => "Identifies the current state of the shipping lock. The SHIPPING_LOCK_STATE_PARTIALLY_LOCKED setting shall be returned when at least one, but not all axes of motion are in the locked state.";

        protected override ERDM_ShippingLockState getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_ShippingLockState>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_ShippingLockState value)
        {
            return Tools.ValueToData(value);
        }

        protected override ERDM_ShippingLockState setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_ShippingLockState>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_ShippingLockState state)
        {
            return Tools.ValueToData(state);
        }
    }
}