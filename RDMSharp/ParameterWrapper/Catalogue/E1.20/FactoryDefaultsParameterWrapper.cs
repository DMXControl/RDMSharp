namespace RDMSharp.ParameterWrapper
{
    public sealed class FactoryDefaultsParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetRequestSetResponse<bool>
    {
        public FactoryDefaultsParameterWrapper() : base(ERDM_Parameter.FACTORY_DEFAULTS)
        {
        }
        public override string Name => "Factory Defaults";
        public override string Description =>
            "This parameter is used to instruct a device to revert to its Factory Default user settings or " +
            "configuration as determined by the Manufacturer.";

        protected override bool getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToBool(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(bool value)
        {
            return Tools.ValueToData(value);
        }
    }
}