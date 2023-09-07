namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class CRMXBridgeModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<bool, bool>, IRDMManufacturerParameterWrapper
    {
        public CRMXBridgeModeParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.CRMX_BRIDGE_MODE)
        {
        }
        public override string Name => "CRMX Bridge Mode";
        public override string Description => "This parameter is used to get and set the CRMX Bridge Mode.";
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