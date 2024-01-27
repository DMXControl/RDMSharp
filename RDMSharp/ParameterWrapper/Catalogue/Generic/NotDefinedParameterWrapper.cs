namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class NotDefinedParameterWrapper : AbstractGenericParameterWrapper<byte[], byte[]>
    {
        public NotDefinedParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }
        protected override byte[] getResponseParameterDataToValue(byte[] parameterData)
        {
            return parameterData;
        }

        protected override byte[] getResponseValueToParameterData(byte[] value)
        {
            return value;
        }

        protected override byte[] setRequestParameterDataToValue(byte[] parameterData)
        {
            return parameterData;
        }

        protected override byte[] setRequestValueToParameterData(byte[] value)
        {
            return value;
        }
    }
}