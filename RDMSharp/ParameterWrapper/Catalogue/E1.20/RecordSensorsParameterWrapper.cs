namespace RDMSharp.ParameterWrapper
{
    public sealed class RecordSensorsParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<byte>
    {
        public RecordSensorsParameterWrapper() : base(ERDM_Parameter.RECORD_SENSORS)
        {
        }
        public override string Name => "Record Sensors";
        public override string Description => "This parameter instructs devices such as dimming racks that monitor load changes to store the current value for monitoring sensor changes.";

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte sensorId)
        {
            return Tools.ValueToData(sensorId);
        }
    }
}