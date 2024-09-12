using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SensorTypeCustomDefinesParameterWrapper : AbstractRDMGetParameterWrapperRanged<byte, RDMSensorTypeCustomDefine>, IRDMBlueprintParameterWrapper
    {
        public SensorTypeCustomDefinesParameterWrapper() : base(ERDM_Parameter.SENSOR_TYPE_CUSTOM)
        {
        }
        public override string Name => "Sensor Type Custom Defines";
        public override string Description => "This parameter is used to obtain a text string that contains a custom friendly name that can be displayed to the user for Manufacturer-Specific Sensor Type Defines. Manufacturer-Specific Sensor Types exist in the range of 0x80-0xFF and shall not duplicate those included in other segments of the list of Sensor Types contained in [RDM] Table A-12.\r\nCustom Sensor Types shall be consistent across all products for a given manufacturer, in that a manufacturer shall not have multiple Defines for the same Sensor Type across different products with the same ESTA Manufacturer ID.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(byte id)
        {
            return Tools.ValueToData(id);
        }
        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override RDMSensorTypeCustomDefine getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSensorTypeCustomDefine.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMSensorTypeCustomDefine sensorTypeCustomDefine)
        {
            return sensorTypeCustomDefine.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            return SensorTypeCustomDefinesParameterWrapper.GetRequestRangeInternal(value);
        }
        internal static IRequestRange GetRequestRangeInternal(object value)
        {
                return new RequestRange<byte>(0x80, 0xff);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType()}");
        }
    }
}