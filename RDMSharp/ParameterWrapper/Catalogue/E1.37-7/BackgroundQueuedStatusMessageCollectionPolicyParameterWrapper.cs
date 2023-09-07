namespace RDMSharp.ParameterWrapper
{
    public sealed class BackgroundQueuedStatusMessageCollectionPolicyParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<GetBackgroundQueuedStatusPolicyResponse, byte>
    {
        public BackgroundQueuedStatusMessageCollectionPolicyParameterWrapper() : base(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY)
        {
        }
        public override string Name => "Background Queued/Status Message Collection Policy";
        public override string Description => 
            "This parameter is used to set a Background Collection Policy for managing collection of Queued " +
            "messages from an RDM device on an E1.20 network.\r\n\r\n" +
            "Background collection policies are a flexible framework for Splitters, Proxies, and RDMnet " +
            "Gateways to implement different configuration policies for collection of Queued and Status " +
            "Messages from RDM Devices. Possible collection strategies might include more frequent " +
            "collection of higher priority messages, or overall higher or lower collection frequencies.";

        protected override GetBackgroundQueuedStatusPolicyResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetBackgroundQueuedStatusPolicyResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetBackgroundQueuedStatusPolicyResponse value)
        {
            return value.ToPayloadData();
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte policy)
        {
            return Tools.ValueToData(policy);
        }
    }
}