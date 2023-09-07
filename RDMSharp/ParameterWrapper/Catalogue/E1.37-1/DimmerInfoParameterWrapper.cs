namespace RDMSharp.ParameterWrapper
{
    public sealed class DimmerInfoParameterWrapper : AbstractRDMGetParameterWrapperEmptyRequest<RDMDimmerInfo>
    {
        public DimmerInfoParameterWrapper() : base(ERDM_Parameter.DIMMER_INFO)
        {
        }
        public override string Name => "Dimmer Info";
        public override string Description => "This parameter is used to retrieve a variety of dimmer related information that describes the capabilities of the dimmer.";

        protected override RDMDimmerInfo getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMDimmerInfo.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMDimmerInfo value)
        {
            return value.ToPayloadData();
        }
    }
}