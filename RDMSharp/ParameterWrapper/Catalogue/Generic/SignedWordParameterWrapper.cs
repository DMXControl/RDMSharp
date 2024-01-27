namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class SignedWordParameterWrapper : AbstractGenericParameterWrapper<short, short>
    {
        public SignedWordParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }

        protected override short getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToShort(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(short value)
        {
            return Tools.ValueToData(value);
        }

        protected override short setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(short value)
        {
            return Tools.ValueToData(value);
        }
    }
}