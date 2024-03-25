using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class ModulationFrequencyDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<byte, RDMModulationFrequencyDescription>, IRDMBlueprintDescriptionListParameterWrapper
    {
        public ModulationFrequencyDescriptionParameterWrapper() : base(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION)
        {
        }
        public override string Name => "Modulation Frequency Description";
        public override string Description =>
            "This parameter is used to get a descriptive ASCII text label for a response time setting. " +
            "The label may be up to 32 characters.";
        public ERDM_Parameter ValueParameterID => ERDM_Parameter.MODULATION_FREQUENCY;

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.MODULATION_FREQUENCY };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override byte getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(byte outputResponseTimeId)
        {
            return Tools.ValueToData(outputResponseTimeId);
        }

        protected override RDMModulationFrequencyDescription getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMModulationFrequencyDescription.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMModulationFrequencyDescription value)
        {
            return value.ToPayloadData();
        }

        public override IRequestRange GetRequestRange(object value)
        {
            if (value is RDMModulationFrequency modulationFrequency)
                return new RequestRange<byte>(1, (byte)(modulationFrequency.Count));
            else if (value == null)
                return new RequestRange<byte>(1, byte.MaxValue);

            throw new NotSupportedException($"There is no support for the Type: {value.GetType().ToString()}");
        }
    }
}