namespace RDMSharp.ParameterWrapper
{
    public sealed class LockStateParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<GetLockStateResponse, SetLockStateRequest>
    {
        public LockStateParameterWrapper() : base(ERDM_Parameter.LOCK_STATE)
        {
        }
        public override string Name => "Lock State";
        public override string Description =>
            "This parameter is used to determine the lock state for devices that support locking.\r\n\r\n" +
            "A lock, when applied, can provide a variable level of access/change protection. " +
            "The locking mechanism is designed to deter tampering and is not intended to provide absolute security.";

        protected override GetLockStateResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetLockStateResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetLockStateResponse value)
        {
            return value.ToPayloadData();
        }

        protected override SetLockStateRequest setRequestParameterDataToValue(byte[] parameterData)
        {
            return SetLockStateRequest.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(SetLockStateRequest setLockState)
        {
            return setLockState.ToPayloadData();
        }
    }
}