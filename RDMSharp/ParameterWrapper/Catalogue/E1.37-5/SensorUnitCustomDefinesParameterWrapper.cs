using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class SensorUnitCustomDefinesParameterWrapper : AbstractRDMGetParameterWrapperRanged<byte, RDMSensorUnitCustomDefine>, IRDMBlueprintParameterWrapper
    {
        public SensorUnitCustomDefinesParameterWrapper() : base(ERDM_Parameter.SENSOR_UNIT_CUSTOM)
        {
        }
        public override string Name => "Sensor Unit Custom Defines";
        public override string Description => "This parameter is used to obtain a text string that contains a custom friendly name that can be displayed to the user for Manufacturer-Specific Sensor Units. Manufacturer-Specific Sensor Units exist in the range of 0x80-0xFF and shall not duplicate those included in other segments of the list of Sensor Unit Defines contained in [RDM] Table A-13.\r\nA Custom Sensor Unit shall be consistent across all products for a given manufacturer, in that a manufacturer shall not have multiple Defines for the same sensor unit across different products with the same ESTA Manufacturer ID.";

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

        protected override RDMSensorUnitCustomDefine getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMSensorUnitCustomDefine.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMSensorUnitCustomDefine sensorUnitCustomDefine)
        {
            return sensorUnitCustomDefine.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            return SensorUnitCustomDefinesParameterWrapper.GetRequestRangeInternal(value);
        }
        internal static IRequestRange GetRequestRangeInternal(object value)
        {
                return new RequestRange<byte>(0x80, 0xff);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType()}");
        }
    }
}