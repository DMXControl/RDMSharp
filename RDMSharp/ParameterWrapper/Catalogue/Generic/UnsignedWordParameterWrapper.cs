namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class UnsignedWordParameterWrapper : AbstractGenericParameterWrapper<ushort, ushort>
    {
        public UnsignedWordParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }


        protected override ushort getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(ushort value)
        {
            return Tools.ValueToData(value);
        }

        protected override ushort setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUShort(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(ushort value)
        {
            return Tools.ValueToData(value);
        }
    }
}