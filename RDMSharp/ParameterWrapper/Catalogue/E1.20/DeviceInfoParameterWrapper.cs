namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceInfoParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMDeviceInfo>, IRDMBlueprintParameterWrapper
    {
        public DeviceInfoParameterWrapper() : base(ERDM_Parameter.DEVICE_INFO)
        {
        }
        public override string Name => "Device Info";
        public override string Description =>
            "This parameter is used to retrieve a variety of information about the device that is normally " +
            "required by a controller.";

        protected override RDMDeviceInfo getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDeviceInfo.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMDeviceInfo deviceInfo)
        {
            return deviceInfo.ToPayloadData();
        }
    }
}