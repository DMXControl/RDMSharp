namespace RDMSharp.ParameterWrapper
{
    public sealed class IdentifyDeviceParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>
    {
        public IdentifyDeviceParameterWrapper() : base(ERDM_Parameter.IDENTIFY_DEVICE)
        {
        }
        public override string Name => "Identify Device";
        public override string Description => "This parameter is used for the user to physically identify the device represented by the UID. " +
            "The responder physically identify itself using a visible or audible action. For example, " +
            "strobing a light or outputting fog.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
        }

        protected override bool setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(bool state)
        {
            return Tools.ValueToData(state);
        }
    }
}