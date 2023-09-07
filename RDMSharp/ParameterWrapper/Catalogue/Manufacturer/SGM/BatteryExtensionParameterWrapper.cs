namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class BatteryExtensionParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EBatteryExtension, EBatteryExtension>, IRDMManufacturerParameterWrapper
    {
        public BatteryExtensionParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.BATTERY_EXTENSION)
        {
        }
        public override string Name => "Battery Extension";
        public override string Description => "This parameter is used to get the Battery Extension hours.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override EBatteryExtension getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EBatteryExtension>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(EBatteryExtension batteryExtension)
        {
            return Tools.ValueToData(batteryExtension);
        }

        protected override EBatteryExtension setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EBatteryExtension>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(EBatteryExtension batteryExtension)
        {
            return Tools.ValueToData(batteryExtension);
        }
    }
}