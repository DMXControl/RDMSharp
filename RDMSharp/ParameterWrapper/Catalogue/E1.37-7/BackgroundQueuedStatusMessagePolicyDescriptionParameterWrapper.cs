namespace RDMSharp.ParameterWrapper
{
    public sealed class BackgroundQueuedStatusMessagePolicyDescriptionParameterWrapper : AbstractRDMGetParameterWrapperRanged<ushort, GetBackgroundQueuedStatusPolicyDescriptionResponse>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public BackgroundQueuedStatusMessagePolicyDescriptionParameterWrapper() : base(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION)
        {
        }
        public override string Name => "Background Queued/Status Message Policy Description";
        public override string Description =>
            "This parameter is used to get a descriptive ASCII text label for a given Background Collection " +
            "Policy Number. The label may be up to 32 characters.";
        public ERDM_Parameter ValueParameterID => ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_LIST };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort endpointId)
        {
            return Tools.ValueToData(endpointId);
        }

        protected override GetBackgroundQueuedStatusPolicyDescriptionResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetBackgroundQueuedStatusPolicyDescriptionResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetBackgroundQueuedStatusPolicyDescriptionResponse value)
        {
            return value.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return EndpointListParameterWrapper.GetRequestRange(value);
        }
    }
}