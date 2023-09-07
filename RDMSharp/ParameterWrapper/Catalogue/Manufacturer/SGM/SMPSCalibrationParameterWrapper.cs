namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class SMPSCalibrationParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>, IRDMManufacturerParameterWrapper
    {
        public SMPSCalibrationParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.SMPS_CALIBRATION)
        {
        }
        public override string Name => "SMPS Calibration";
        public override string Description => "This parameter is used to recalibrate the SMPS.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool crmxBridgeMode)
        {
            return Tools.ValueToData(crmxBridgeMode);
        }

        protected override bool setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(bool crmxBridgeMode)
        {
            return Tools.ValueToData(crmxBridgeMode);
        }
    }
}