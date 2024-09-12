using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class MetadataJsonParameterWrapper : AbstractRDMGetParameterWrapper<ERDM_Parameter, RDMMetadataJson>, IRDMBlueprintParameterWrapper
    {
        public MetadataJsonParameterWrapper() : base(ERDM_Parameter.METADATA_JSON)
        {
        }
        public override string Name => "Metadata JSON";
        public override string Description => "This parameter is used to request the Parameter Metadata Language description for a Manufacturer-Specific Parameter Message.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.SUPPORTED_PARAMETERS };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(ERDM_Parameter parameter)
        {
            return Tools.ValueToData(parameter);
        }
        protected override ERDM_Parameter getRequestParameterDataToValue(byte[] parameterData)
        {
            return (ERDM_Parameter)Tools.DataToUShort(ref parameterData);
        }

        protected override RDMMetadataJson getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMMetadataJson.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMMetadataJson metadataJson)
        {
            return metadataJson.ToPayloadData();
        }
    }
}