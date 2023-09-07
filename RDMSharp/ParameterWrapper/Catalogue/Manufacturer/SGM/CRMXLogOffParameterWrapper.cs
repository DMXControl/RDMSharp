namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class CRMXLogOffParameterWrapper : AbstractRDMSetParameterWrapperEmptyRequestResponse, IRDMManufacturerParameterWrapper
    {
        public CRMXLogOffParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.CRMX_LOG_OFF)
        {
        }
        public override string Name => "CRMX Log Off";
        public override string Description => "Gets the info, if the fixture is connected to CRMX.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;
    }
}