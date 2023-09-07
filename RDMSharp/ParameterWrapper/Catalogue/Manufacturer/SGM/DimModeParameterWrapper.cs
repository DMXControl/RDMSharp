namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class DimModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EDimMode, EDimMode>, IRDMManufacturerParameterWrapper
    {
        public DimModeParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.DIM_MODE)
        {
        }
        public override string Name => "Dim Mode";
        public override string Description => "This parameter is used to retrieve or set the Dim Mode. " +
            "In the mode 'Standard', the colors dim in relation to the temperature of the fixture, to keep the color output constant. " +
            "In the mode 'Max Power', the output is not adjusted in relation to the temperature of the fixture.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override EDimMode getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EDimMode>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(EDimMode dimMode)
        {
            return Tools.ValueToData(dimMode);
        }

        protected override EDimMode setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EDimMode>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(EDimMode dimMode)
        {
            return Tools.ValueToData(dimMode);
        }
    }
}