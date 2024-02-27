namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class RefreshRateParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RefreshRate, RefreshRate>, IRDMManufacturerParameterWrapper
    {
        public RefreshRateParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.REFRESH_RATE)
        {
        }
        public override string Name => "Refresh Rate";
        public override string Description => "This parameter is used to retrieve or set the Refresh Rate. " +
            "'Dimmer optimized' sets the LED’s to a dimmer optimized default. (factory default) " +
            "'High frequency optimized' sets the LED’s to a high frequency optimized default. " +
            "'Custom value' sets a custom frequency (refresh rate) for the LED’s.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override RefreshRate getResponseParameterDataToValue(byte[] parameterData)
        {
            return new RefreshRate(Tools.DataToByte(ref parameterData));
        }

        protected override byte[] getResponseValueToParameterData(RefreshRate refreshRate)
        {
            return [refreshRate.RawValue];
        }

        protected override RefreshRate setRequestParameterDataToValue(byte[] parameterData)
        {
            return new RefreshRate(Tools.DataToByte(ref parameterData));
        }

        protected override byte[] setRequestValueToParameterData(RefreshRate refreshRate)
        {
            return [refreshRate.RawValue];
        }
    }
}