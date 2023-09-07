namespace RDMSharp.ParameterWrapper.SGM
{
    // JPK: Commented because there is already a Wrapper for DMX_FAIL_MODE
    //public sealed class DMXFailModeModeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EDMXFailMode, EDMXFailMode>, IRDMManufacturerParameterWrapper
    //{
    //    public DMXFailModeModeParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.DMX_FAIL_MODE)
    //    {
    //    }
    //    public override string Name => "DMX Fail Mode";
    //    public override string Description => "This parameter is used to retrieve or set the DMX Fail Mode.";
    //    public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

    //    protected override EDMXFailMode getResponseParameterDataToValue(byte[] parameterData)
    //    {
    //        return Tools.DataToEnum<EDMXFailMode>(ref parameterData);
    //    }

    //    protected override byte[] getResponseValueToParameterData(EDMXFailMode dmxFailMode)
    //    {
    //        return Tools.ValueToData(dmxFailMode);
    //    }

    //    protected override EDMXFailMode setRequestParameterDataToValue(byte[] parameterData)
    //    {
    //        return Tools.DataToEnum<EDMXFailMode>(ref parameterData);
    //    }

    //    protected override byte[] setRequestValueToParameterData(EDMXFailMode dmxFailMode)
    //    {
    //        return Tools.ValueToData(dmxFailMode);
    //    }
    //}
}