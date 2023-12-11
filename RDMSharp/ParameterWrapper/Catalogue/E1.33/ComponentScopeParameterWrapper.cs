namespace RDMSharp.ParameterWrapper
{
    public sealed class ComponentScopeParameterWrapper : AbstractRDMGetSetParameterWrapperEmptySetResponse<ushort, GetSetComponentScope, GetSetComponentScope>
    {
        public ComponentScopeParameterWrapper() : base(ERDM_Parameter.COMPONENT_SCOPE)
        {
        }
        public override string Name => "Component Scope";
        public override string Description =>
            "This parameter is used to read and modify the Scopes and static Broker configurations of a Component.\r\n\r\n" +
            "Each Component that supports this parameter maintain a Scope List, numbered " +
            "sequentially, starting with one (0x0001). Devices and Brokers only support a single Scope and, " +
            "thus, only have a single - element list. Controllers support multiple Scopes and thus have " +
            "multiple elements in the Scope List.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_LIST };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort scopeSlot)
        {
            return Tools.ValueToData(scopeSlot);
        }

        protected override GetSetComponentScope getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetSetComponentScope.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetSetComponentScope componentScope)
        {
            return componentScope.ToPayloadData();
        }

        protected override GetSetComponentScope setRequestParameterDataToValue(byte[] parameterData)
        {
            return GetSetComponentScope.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(GetSetComponentScope componentScope)
        {
            return componentScope.ToPayloadData();
        }
        public override RequestRange<ushort> GetRequestRange(object value)
        {
            return EndpointListParameterWrapper.GetRequestRange(value);
        }
    }
}