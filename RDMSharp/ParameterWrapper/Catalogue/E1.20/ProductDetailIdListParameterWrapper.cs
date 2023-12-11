using System.Collections.Generic;

namespace RDMSharp.ParameterWrapper
{
    public sealed class ProductDetailIdListParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<ERDM_ProductDetail[]>
    {
        public ProductDetailIdListParameterWrapper() : base(ERDM_Parameter.PRODUCT_DETAIL_ID_LIST)
        {
        }
        public override string Name => "Product Detail ID List";
        public override string Description =>
            "This parameter is used for requesting technology details for a device. The response is a " +
            "packed message containing up to six product detail identifications.";

        protected override ERDM_ProductDetail[] getResponseParameterDataToValue(byte[] parameterData)
        {
            List<ERDM_ProductDetail> productDetails = new List<ERDM_ProductDetail>();
            while (parameterData.Length >= 2)
                productDetails.Add(Tools.DataToEnum<ERDM_ProductDetail>(ref parameterData));

            return productDetails.ToArray();
        }

        protected override byte[] getResponseValueToParameterData(ERDM_ProductDetail[] value)
        {
            List<byte> bytes = new List<byte>();
            foreach (var item in value)
                bytes.AddRange(Tools.ValueToData(item));

            return bytes.ToArray();
        }
    }
}