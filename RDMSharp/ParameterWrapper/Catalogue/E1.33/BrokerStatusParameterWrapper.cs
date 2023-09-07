namespace RDMSharp.ParameterWrapper
{
    public sealed class BrokerStatusParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<GetBrokerStatusResponse, ERDM_BrokerStatus>
    {
        public BrokerStatusParameterWrapper() : base(ERDM_Parameter.BROKER_STATUS)
        {
        }
        public override string Name => "Broker Status";
        public override string Description => "This parameter is used to read and modify the status of a Broker.";

        protected override GetBrokerStatusResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetBrokerStatusResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetBrokerStatusResponse value)
        {
            return value.ToPayloadData();
        }

        protected override ERDM_BrokerStatus setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_BrokerStatus>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_BrokerStatus brockerState)
        {
            return Tools.ValueToData(brockerState);
        }
    }
}