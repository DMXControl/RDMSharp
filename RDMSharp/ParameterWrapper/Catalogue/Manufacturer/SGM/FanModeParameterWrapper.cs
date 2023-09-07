namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class FanModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EFanMode, EFanMode>, IRDMManufacturerParameterWrapper
    {
        public FanModeParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.FAN_MODE)
        {
        }
        public override string Name => "Fan Mode";
        public override string Description => "This parameter is used to retrieve or set the Fan Mode. " +
            "'Auto' sets the fan speed automatically by the fixture. " +
            "'Standard' adjusts the fan speed relative to internal fixture temperature. " +
            "'Silent' sets low fan speed for quiet operation. " +
            "'Max Power' sets high fan speed for maximum cooling effect.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override EFanMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EFanMode>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(EFanMode fanMode)
        {
            return Tools.ValueToData(fanMode);
        }

        protected override EFanMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EFanMode>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(EFanMode fanMode)
        {
            return Tools.ValueToData(fanMode);
        }
    }
}