using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SensorDefinitionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMSensorDefinition>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public SensorDefinitionParameterWrapper() : base(ERDM_Parameter.SENSOR_DEFINITION)
        {
        }
        public override string Name => "Sensor Definition";
        public override string Description =>
            "This parameter is used to retrieve the definition of a specific sensor. When this parameter is " +
            "directed to a Sub - Device, the reply is identical for any given sensor number in all Sub - " +
            "Devices owned by a specific Root Device.";

        public ERDM_Parameter ValueParameterID => ERDM_Parameter.SENSOR_VALUE;

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

        protected override RDMSensorDefinition getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSensorDefinition.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMSensorDefinition sensorDefinition)
        {
            return sensorDefinition.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            if (value is RDMDeviceInfo deviceInfo)
                return new RequestRange<byte>(0, (byte)(deviceInfo.SensorCount - 1));
            else if (value == null)
                return new RequestRange<byte>(0, byte.MaxValue - 1);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType()}");
        }
    }
}