namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class NorthCalibrationParameterWrapper : AbstractRDMSetParameterWrapperEmptyRequestResponse, IRDMManufacturerParameterWrapper
    {
        public NorthCalibrationParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.NORTH_CALIBRATION)
        {
        }
        public override string Name => "North Calibration";
        public override string Description => "This parameter is used to resets the north position.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;
    }
}