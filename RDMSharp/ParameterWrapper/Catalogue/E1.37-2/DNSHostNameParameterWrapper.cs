using System;

namespace RDMSharp.ParameterWrapper
{
    public sealed class DNSHostNameParameterWrapper : AbstractRDMGetSetParameterWrapperEmptyGetRequestSetResponse<string, string>
    {
        public DNSHostNameParameterWrapper() : base(ERDM_Parameter.DNS_HOSTNAME)
        {
        }
        public override string Name => "Host Name";
        public override string Description => "This parameter is used to get and set the unqualified host name for a device.";

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value is not Valid");
            if (value.Length > 63)
                throw new ArgumentException("value is to long (63 chars are allowed)");

            return Tools.ValueToData(value, trim: 63);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value is not Valid");
            if (value.Length > 63)
                throw new ArgumentException("value is to long (63 chars are allowed)");

            return Tools.ValueToData(value, trim: 63);
        }
    }
}