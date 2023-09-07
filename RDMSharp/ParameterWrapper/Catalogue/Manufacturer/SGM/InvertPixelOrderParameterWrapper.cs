namespace RDMSharp.ParameterWrapper.SGM
{
    public sealed class InvertPixelOrderParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<EInvertPixelOrder, EInvertPixelOrder>, IRDMManufacturerParameterWrapper
    {
        public InvertPixelOrderParameterWrapper() : base((ERDM_Parameter)(ushort)EParameter.INVERT_PIXEL_ORDER)
        {
        }
        public override string Name => "Invert Pixel Order";
        public override string Description => "This parameter is used to retrieve or set the Pixel Order. " +
            "'Standard' is the default order. 'Invert' switches the pixel order of the device.";
        public EManufacturer Manufacturer => EManufacturer.SGM_Technology_For_Lighting_SPA;

        protected override EInvertPixelOrder getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EInvertPixelOrder>(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(EInvertPixelOrder pixelOrder)
        {
            return Tools.ValueToData(pixelOrder);
        }

        protected override EInvertPixelOrder setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<EInvertPixelOrder>(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(EInvertPixelOrder pixelOrder)
        {
            return Tools.ValueToData(pixelOrder);
        }
    }
}