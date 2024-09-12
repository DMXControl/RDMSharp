namespace RDMSharp.ParameterWrapper
{
    public sealed class BindingAndControlFieldsParameterWrapper : AbstractRDMGetParameterWrapperRanged<GetBindingAndControlFieldsRequest, GetBindingAndControlFieldsResponse>
    {
        public BindingAndControlFieldsParameterWrapper() : base(ERDM_Parameter.BINDING_CONTROL_FIELDS)
        {
        }
        public override string Name => "Binding and Control Fields";
        public override string Description =>
            "This parameter allows a Controller to retrieve the Binding UID and Control Field information that " +
            "is sent as part of the Discovery Mute(Disc Mute) message. This allows an RDMnet Device to retrieve these fields that are not " +
            "otherwise available within RDMnet. The Disc Mute message is disallowed within RDMnet.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[] { ERDM_Parameter.ENDPOINT_LIST };
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override GetBindingAndControlFieldsRequest getRequestParameterDataToValue(byte[] parameterData)
        {
            return GetBindingAndControlFieldsRequest.FromPayloadData(parameterData);
        }

        protected override byte[] getRequestValueToParameterData(GetBindingAndControlFieldsRequest request)
        {
            return request.ToPayloadData();
        }

        protected override GetBindingAndControlFieldsResponse getResponseParameterDataToValue(byte[] parameterData)
        {
            return GetBindingAndControlFieldsResponse.FromPayloadData(parameterData);
        }

        protected override byte[] getResponseValueToParameterData(GetBindingAndControlFieldsResponse response)
        {
            return response.ToPayloadData();
        }
        public override IRequestRange GetRequestRange(object value)
        {
            throw new System.NotSupportedException();
        }
    }
}