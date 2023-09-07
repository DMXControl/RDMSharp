namespace RDMSharp.ParameterWrapper
{
    public sealed class SubDeviceStatusReportThresholdParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<ERDM_Status, ERDM_Status>
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.RANGE_0X0001_0x0200;
        public override ERDM_SupportedSubDevice SupportedSetSubDevices => ERDM_SupportedSubDevice.ALL_EXCEPT_ROOT;
        public SubDeviceStatusReportThresholdParameterWrapper() : base(ERDM_Parameter.SUB_DEVICE_STATUS_REPORT_THRESHOLD)
        {
        }
        public override string Name => "Sub-Device Status Reporting Threshold";
        public override string Description => 
            "This parameter is used to set the verbosity of Sub-Device reporting using the Status Type codes. " +
            "This feature is used to inhibit reports from, for example, a specific dimmer in a rack that is generating repeated errors.";

        protected override ERDM_Status getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_Status>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ERDM_Status status)
        {
            return Tools.ValueToData(status);
        }

        protected override ERDM_Status setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_Status>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ERDM_Status status)
        {
            return Tools.ValueToData(status);
        }
    }
}