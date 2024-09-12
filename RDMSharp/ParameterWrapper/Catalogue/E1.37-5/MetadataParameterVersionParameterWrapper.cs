using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class MetadataParameterVersionParameterWrapper : AbstractRDMGetParameterWrapper<ERDM_Parameter, RDMMetadataParameterVersion>, IRDMBlueprintParameterWrapper
    {
        public MetadataParameterVersionParameterWrapper() : base(ERDM_Parameter.METADATA_PARAMETER_VERSION)
        {
        }
        public override string Name => "Metadata Parameter Version";
        public override string Description => "This parameter is used to get the version information for Parameter Messages for which the Responder has descriptions in the Parameter Metadata Language form.\r\nThis Parameter Message can be used by Controllers that may cache Parameter Metadata Language descriptions, to help determine whether or not to fetch the full Parameter Metadata Language description for each Manufacturer-Specific supported parameter.";

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

        protected override RDMMetadataParameterVersion getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMMetadataParameterVersion.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMMetadataParameterVersion metadataParameterVersion)
        {
            return metadataParameterVersion.ToPayloadData();
        }
    }
}