namespace RDMSharp.ParameterWrapper
{
    public sealed class DeviceInfoOffstageParameterWrapper : AbstractRDMGetParameterWrapper<GetDeviceInfoOffstageRequest,GetDeviceInfoOffstageResponse>,IRDMBlueprintParameterWrapper
    {
        public DeviceInfoOffstageParameterWrapper() : base(ERDM_Parameter.DEVICE_INFO_OFFSTAGE)
        {
        }
        public override string Name => "Device Info Offstage";
        public override string Description => "This parameter returns the Device Info dataset for the requested Sub-Device and Personality, without having to switch the Responder into the specific personality. It allows the Device Information for a particular personality to be retrieved while remaining in the current Personality.\nGet Device Information can be used with the Root Device or with Sub-Devices. However, since the personality of the Root Device can change the number of Sub-Devices present, the Sub-Device field in the RDM header shall always be set to the Root Device. The Parameter Data indicates which Sub-Device the request is intended for.\nThe Sub-Device in the Parameter data references the Sub-Device in the selected personality and not the currently active personality, if they are different.";

        public override ERDM_Parameter[] DescriptiveParameters => throw new System.NotImplementedException();

        protected override GetDeviceInfoOffstageRequest getRequestParameterDataToValue(byte[] parameterData)
        {
            return GetDeviceInfoOffstageRequest.FromPayloadData(parameterData);
        }

        protected override byte[] getRequestValueToParameterData(GetDeviceInfoOffstageRequest value)
        {
            return Tools.ValueToData(value);
        }

        protected override GetDeviceInfoOffstageResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetDeviceInfoOffstageResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetDeviceInfoOffstageResponse value)
        {
            return Tools.ValueToData(value);
        }
    }
}