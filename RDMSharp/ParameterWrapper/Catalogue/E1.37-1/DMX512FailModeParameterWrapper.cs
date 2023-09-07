namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512FailModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMDMX_xxxx_Mode, RDMDMX_xxxx_Mode>
    {
        public DMX512FailModeParameterWrapper() : base(ERDM_Parameter.DMX_FAIL_MODE)
        {
        }
        public override string Name => "DMX512 Fail Mode";
        public override string Description => 
            "This parameter defines the behavior of the device when the DMX512 control signal is lost.\r\n\r\n" + 
            "A scene that is triggered by a DMX512 Loss of Signal condition usually ignores the Wait Time stored " +
            "using the CAPTURE_PRESET PID and instead use the Hold Time included with this PID.";

        protected override RDMDMX_xxxx_Mode getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDMX_xxxx_Mode.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMDMX_xxxx_Mode dmxFailMode)
        {
            return Tools.ValueToData(dmxFailMode);
        }

        protected override RDMDMX_xxxx_Mode setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMDMX_xxxx_Mode.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMDMX_xxxx_Mode dmxFailMode)
        {
            return Tools.ValueToData(dmxFailMode);
        }
    }
}