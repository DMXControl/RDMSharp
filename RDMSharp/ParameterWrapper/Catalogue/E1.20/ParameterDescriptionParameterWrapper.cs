using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class ParameterDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<ERDM_Parameter, RDMParameterDescription>, IRDMBlueprintParameterWrapper
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public ParameterDescriptionParameterWrapper() : base(ERDM_Parameter.PARAMETER_DESCRIPTION)
        {
        }
        public override string Name => "Parameter Description";
        public override string Description =>
            "This parameter is used to retrieve the definition of some manufacturer-specific PIDs. The purpose" +
            "of this parameter is to allow a controller to retrieve enough information about the manufacturer -" +
            "specific PID to generate Get and Set commands.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.SUPPORTED_PARAMETERS };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte[] getRequestValueToParameterData(ERDM_Parameter parameterID)
        {
            return Tools.ValueToData(parameterID);
        }
        protected override ERDM_Parameter getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToEnum<ERDM_Parameter>(ref parameterData);
        }

        protected override RDMParameterDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMParameterDescription.FromPayloadData(parameterData);
        }
        protected override byte[] getResponseValueToParameterData(RDMParameterDescription parameterDescription)
        {
            return parameterDescription.ToPayloadData();
        }

        public override RequestRange<ERDM_Parameter> GetRequestRange(object value)
        {
            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}