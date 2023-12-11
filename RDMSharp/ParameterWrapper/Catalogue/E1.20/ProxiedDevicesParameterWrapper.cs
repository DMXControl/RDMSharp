namespace RDMSharp.ParameterWrapper
{
    public sealed class ProxiedDevicesParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMProxiedDevices>
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public ProxiedDevicesParameterWrapper() : base(ERDM_Parameter.PROXIED_DEVICES)
        {
        }
        public override string Name => "Proxied Devices";
        public override string Description =>
            "This parameter is used to retrieve the UIDs from a device identified as a proxy during discovery. " +
            "The response to this parameter contains a packed list of 48 - bit UIDs for all devices represented " +
            "by the proxy.";

        protected override RDMProxiedDevices getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMProxiedDevices.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMProxiedDevices value)
        {
            return value.ToPayloadData();
        }
    }
}