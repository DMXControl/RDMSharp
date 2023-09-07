namespace RDMSharp.ParameterWrapper
{
    public sealed class ClientSearchDomainParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<string, string>
    {
        public ClientSearchDomainParameterWrapper() : base(ERDM_Parameter.SEARCH_DOMAIN)
        {
        }
        public override string Name => "Client Search Domain";
        public override string Description => 
            "This parameter is used to read and modify the DNS search domain name configuration used by " +
            "Clients when locating a Broker.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData, 231);
        }

        protected override byte[] getResponseValueToParameterData(string dnsDomainName)
        {
            return Tools.ValueToData(dnsDomainName, 231);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData, 231);
        }

        protected override byte[] setRequestValueToParameterData(string dnsDomainName)
        {
            return Tools.ValueToData(dnsDomainName, 231);
        }
    }
}