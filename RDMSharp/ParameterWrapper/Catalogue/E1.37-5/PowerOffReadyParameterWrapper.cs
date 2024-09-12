namespace RDMSharp.ParameterWrapper
{
    public sealed class PowerOffReadyParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<bool>
    {
        public PowerOffReadyParameterWrapper() : base(ERDM_Parameter.POWER_OFF_READY)
        {
        }
        public override string Name => "Power Off Ready";
        public override string Description => "This parameter is for devices that require a shutdown period so that they can enter an appropriate state before power is physically removed from the device.\r\nThis parameter indicates that the device is in an appropriate state such that power can be removed from the device without causing an issue.\r\nThis parameter may be used with the POWER_STATE message from Section 10.11.3 of [RDM], where the POWER_STATE message is used to initiate the shutdown. The POWER_OFF_READY parameter message can be used to determine if it is now appropriate to remove power from the device.\r\nDepending on the device, once a shutdown is initiated, the device may not be able to respond to any other messages. If the device is still able to respond after completing any shutdown processes, then it should queue a POWER_OFF_READY message.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
        }
    }
}