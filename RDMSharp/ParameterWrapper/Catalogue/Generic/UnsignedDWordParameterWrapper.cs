namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class UnsignedDWordParameterWrapper : AbstractGenericParameterWrapper<uint, uint>
    {
        public UnsignedDWordParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }
        protected override uint getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }

        protected override uint setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToUInt(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(uint value)
        {
            return Tools.ValueToData(value);
        }
    }
}