namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class SignedDWordParameterWrapper : AbstractGenericParameterWrapper<int, int>
    {
        public SignedDWordParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }

        protected override int getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(int value)
        {
            return Tools.ValueToData(value);
        }

        protected override int setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(int value)
        {
            return Tools.ValueToData(value);
        }
    }
}