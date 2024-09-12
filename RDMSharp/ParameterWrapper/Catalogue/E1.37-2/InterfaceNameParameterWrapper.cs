namespace RDMSharp.ParameterWrapper
{
    public sealed class InterfaceNameParameterWrapper : AbstractRDMGetParameterWrapperRanged<uint, GetInterfaceNameResponse>
    {
        public InterfaceNameParameterWrapper() : base(ERDM_Parameter.INTERFACE_LABEL)
        {
        }
        public override string Name => "Interface Name";
        public override string Description => "This parameter is used to retrieve the label for a network interface.";
        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.LIST_INTERFACES };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override uint getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(uint interfaceId)
        {
            return Tools.ValueToData(interfaceId);
        }

        protected override GetInterfaceNameResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetInterfaceNameResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetInterfaceNameResponse value)
        {
            return value.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return ListInterfacesParameterWrapper.GetRequestRange(value);
        }
    }
}