namespace RDMSharp.ParameterWrapper
{
    public sealed class StatusIdDescriptionParameterWrapper : AbstractRDMGetParameterWrapper<ushort, string>, IRDMBlueprintParameterWrapper
    {
        public override ERDM_SupportedSubDevice SupportedGetSubDevices => ERDM_SupportedSubDevice.ROOT;
        public StatusIdDescriptionParameterWrapper() : base(ERDM_Parameter.STATUS_ID_DESCRIPTION)
        {
        }
        public override string Name => "Status ID Description";
        public override string Description => "This parameter is used to request an ASCII text description of a given Status ID. The description may be up to 32 characters.";

        private static readonly ERDM_Parameter[] descriptiveParameters = new ERDM_Parameter[0];
        public override ERDM_Parameter[] DescriptiveParameters => descriptiveParameters;

        protected override ushort getRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getRequestValueToParameterData(ushort statusID)
        {
            return Tools.ValueToData(statusID);
        }

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }
        public override IRequestRange GetRequestRange(object value)
        {
            return new RequestRange<ushort>(0, ushort.MaxValue);
        }
    }
}