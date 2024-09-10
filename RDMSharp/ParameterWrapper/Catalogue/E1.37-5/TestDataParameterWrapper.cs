namespace RDMSharp.ParameterWrapper
{
    public sealed class TestDataParameterWrapper : AbstractRDMParameterWrapper<ushort, byte[], byte[], byte[]>
    {
        public override sealed ERDM_CommandClass CommandClass => ERDM_CommandClass.GET | ERDM_CommandClass.SET;
        public TestDataParameterWrapper() : base(ERDM_Parameter.TEST_DATA)
        {
        }
        public override string Name => "Test Data";
        public override string Description => "This parameter is used to send RDM packets with a specific size and payload. It can be used to validate network data integrity, to test Responder packet handling, and for other troubleshooting and development operations.\r\nThis parameter should have no effect on Responder operation besides producing the RDM response.\r\nResponders are encouraged to support the full range of allowed PDLs for both GET_COMMAND_RESPONSE and SET_COMMAND_RESPONSE messages. If a Responder receives a message with a Pattern Length larger than it supports, it shall respond with a NACK Reason Code of NR_DATA_OUT_OF_RANGE.";

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort value)
        {
            return Tools.ValueToData(value);
        }

        protected override byte[] getResponseParameterDataToValue(byte[] parameterData)
        {
            return parameterData;
        }

        protected override byte[] getResponseValueToParameterData(byte[] value)
        {
            return value;
        }

        protected override byte[] setRequestParameterDataToValue(byte[] parameterData)
        {
            return parameterData;
        }

        protected override byte[] setRequestValueToParameterData(byte[] value)
        {
            return value;
        }

        protected override byte[] setResponseParameterDataToValue(byte[] parameterData)
        {
            return parameterData;
        }

        protected override byte[] setResponseValueToParameterData(byte[] value)
        {
            return value;
        }
    }
}