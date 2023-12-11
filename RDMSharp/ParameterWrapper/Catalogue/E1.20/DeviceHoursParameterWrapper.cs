namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceHoursParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<uint, uint>
    {
        public DeviceHoursParameterWrapper() : base(ERDM_Parameter.DEVICE_HOURS)
        {
        }
        public override string Name => "Device Hours";
        public override string Description =>
            "This parameter is used to retrieve or set the number of hours of operation the device has been in " +
            "use. Some devices may only support the retrieval of this parameter and not allow the " +
            "device’s hours to be set.";

        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }

        protected override uint setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(uint deviceHours)
        {
            return Tools.ValueToData(deviceHours);
        }
    }
}