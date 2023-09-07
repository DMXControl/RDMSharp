namespace RDMSharp.ParameterWrapper
{
    public sealed class ResetDeviceParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<ERDM_ResetType>
    {
        public ResetDeviceParameterWrapper() : base(ERDM_Parameter.RESET_DEVICE)
        {
        }
        public override string Name => "Reset Device";
        public override string Description => "This parameter is used to instruct the responder to reset itself. This parameter also clears the Discovery Mute flag. A cold reset is the equivalent of removing and reapplying power to the device.";

        protected override ERDM_ResetType setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_ResetType>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_ResetType resetType)
        {
            return Tools.ValueToData(resetType);
        }
    }
}