using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class DNSDomainNameParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<string, string>
    {
        public DNSDomainNameParameterWrapper() : base(ERDM_Parameter.DNS_DOMAIN_NAME)
        {
        }
        public override string Name => "Domain Name";
        public override string Description => "This parameter is used to get and set the unqualified domain name for a device.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value is not Valid");
            if (value.Length > 231)
                throw new ArgumentException("value is to long (231 chars are allowed)");

            return Tools.ValueToData(value, trim: 213);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value is not Valid");
            if (value.Length > 231)
                throw new ArgumentException("value is to long (231 chars are allowed)");

            return Tools.ValueToData(value, trim: 213);
        }
    }
}