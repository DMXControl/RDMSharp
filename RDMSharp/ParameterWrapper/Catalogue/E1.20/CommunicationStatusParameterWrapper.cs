namespace RDMSharp.ParameterWrapper
{
    public sealed class CommunicationStatusParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetRequestSetResponse<RDMCommunicationStatus>
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public CommunicationStatusParameterWrapper() : base(ERDM_Parameter.COMMS_STATUS)
        {
        }

        public override string Name => "Communication Status";
        public override string Description => "The Communication Status parameter is used to collect information that may be useful in analyzing the integrity of the communication system.";

        protected override RDMCommunicationStatus getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMCommunicationStatus.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMCommunicationStatus value)
        {
            return value.ToPayloadData();
        }
    }
}