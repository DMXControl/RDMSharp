namespace RDMSharp.ParameterWrapper
{
    public sealed class ModulationFrequencyParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<RDMModulationFrequency, byte>
    {
        public ModulationFrequencyParameterWrapper() : base(ERDM_Parameter.MODULATION_FREQUENCY)
        {
        }
        public override string Name => "Modulation Frequency";
        public override string Description => "This parameter is used to get and set the modulation frequency for devices that support adjustment of the modulation frequency of their output.";
        protected override RDMModulationFrequency getResponseParameterDataToValue(byte[] parameterData)
        {
            return RDMModulationFrequency.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(RDMModulationFrequency value)
        {
            return value.ToPayloadData();
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte modulationFrequencyId)
        {
            return Tools.ValueToData(modulationFrequencyId);
        }
    }
}