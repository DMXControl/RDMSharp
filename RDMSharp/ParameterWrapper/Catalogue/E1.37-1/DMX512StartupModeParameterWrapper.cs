namespace RDMSharp.ParameterWrapper
{
    public sealed class DMX512StartupModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMDMX_xxxx_Mode, RDMDMX_xxxx_Mode>
    {
        public DMX512StartupModeParameterWrapper() : base(ERDM_Parameter.DMX_STARTUP_MODE)
        {
        }
        public override string Name => "DMX512 Startup Mode";
        public override string Description => 
            "This parameter defines the behavior of the device when it starts up and the DMX512 control signal is absent.\r\n" +
            "The DMX512 signal is considered absent when standard DMX packets are not received for a period of greater than 1.25 seconds.";

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