namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class ASCIIParameterWrapper : AbstractGenericParameterWrapper<string, string>
    {
        public ASCIIParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }

        protected override string getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }

        protected override string setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToString(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(string value)
        {
            return Tools.ValueToData(value);
        }
    }
}