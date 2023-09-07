using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SensorValueParameterWrapper : AbstractRDMGetSetParameterWrapper<byte, RDMSensorValue, byte, RDMSensorValue>
    {
        public SensorValueParameterWrapper() : base(ERDM_Parameter.SENSOR_VALUE)
        {
        }
        public override string Name => "Sensor Value";
        public override string Description => "This parameter shall be used to retrieve or reset sensor data.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.DEVICE_INFO };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(byte sensorId)
        {
            return Tools.ValueToData(sensorId);
        }
        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override RDMSensorValue getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSensorValue.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMSensorValue sensorValue)
        {
            return sensorValue.ToPayloadData();
        }

        protected override byte[] setRequestValueToParameterData(byte sensorId)
        {
            return Tools.ValueToData(sensorId);
        }

        protected override RDMSensorValue setResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSensorValue.FromPayloadData(parameterData);
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setResponseValueToParameterData(RDMSensorValue value)
        {
            return value.ToPayloadData();
        }

        public override RequestRange<byte> GetRequestRange(object value)
        {
            if (value is RDMDeviceInfo deviceInfo)
                return new RequestRange<byte>(0, (byte)(deviceInfo.SensorCount - 1));
            else if (value == null)
                return new RequestRange<byte>(0, byte.MaxValue - 1);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}