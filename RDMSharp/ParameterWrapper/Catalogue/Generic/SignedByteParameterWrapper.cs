namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class SignedByteParameterWrapper : AbstractGenericParameterWrapper<sbyte, sbyte>
    {
        public SignedByteParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }
        protected override sbyte getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToSByte(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(sbyte value)
        {
            return Tools.ValueToData(value);
        }

        protected override sbyte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToSByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(sbyte value)
        {
            return Tools.ValueToData(value);
        }
    }
}