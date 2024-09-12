namespace RDMSharp.ParameterWrapper
{
    public sealed class CommunicationStatusNullStartCodeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetRequestSetResponse<GetCommunicationStatusNullStartCodeResponse>,IRDMBlueprintParameterWrapper
    {
        public CommunicationStatusNullStartCodeParameterWrapper() : base(ERDM_Parameter.COMMS_STATUS_NSC)
        {
        }
        public override string Name => "Communication Status Null Start Code";
        public override string Description => "This parameter allows a Controller to retrieve statistical packet and error counters relating to NULL START Code (NSC) packets (see [DMX], Section 8.5.1).";

        protected override GetCommunicationStatusNullStartCodeResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetCommunicationStatusNullStartCodeResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetCommunicationStatusNullStartCodeResponse value)
        {
            return Tools.ValueToData(value);
        }
    }
}