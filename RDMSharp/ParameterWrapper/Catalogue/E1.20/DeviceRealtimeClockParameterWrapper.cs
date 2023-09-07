namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceRealtimeClockParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMRealTimeClock, RDMRealTimeClock>
    {
        public DeviceRealtimeClockParameterWrapper() : base(ERDM_Parameter.REAL_TIME_CLOCK)
        {
        }
        public override string Name => "Device Real-Time Clock";
        public override string Description => "This parameter is used to retrieve or set the real-time clock in a device.";

        protected override RDMRealTimeClock getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMRealTimeClock.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMRealTimeClock value)
        {
            return value.ToPayloadData();
        }

        protected override RDMRealTimeClock setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMRealTimeClock.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMRealTimeClock realTimeClock)
        {
            return realTimeClock.ToPayloadData();
        }
    }
}