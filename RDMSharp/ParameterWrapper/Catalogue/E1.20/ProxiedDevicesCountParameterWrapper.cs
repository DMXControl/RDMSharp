namespace RDMSharp.ParameterWrapper
{
    public sealed class ProxiedDevicesCountParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMProxiedDeviceCount>
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public ProxiedDevicesCountParameterWrapper() : base(ERDM_Parameter.PROXIED_DEVICES_COUNT)
        {
        }
        public override string Name => "Proxied Devices Count";
        public override string Description => 
            "This parameter is used to identify the number of devices being represented by a proxy and " +
            "whether the list of represented device UIDs has changed.";

        protected override RDMProxiedDeviceCount getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMProxiedDeviceCount.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMProxiedDeviceCount value)
        {
            return value.ToPayloadData();
        }
    }
}