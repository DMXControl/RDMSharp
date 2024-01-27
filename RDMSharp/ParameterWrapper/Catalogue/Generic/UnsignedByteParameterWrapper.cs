namespace RDMSharp.ParameterWrapper.Generic
{
    public sealed class UnsignedByteParameterWrapper : AbstractGenericParameterWrapper<byte, byte>, IRDMGetParameterWrapperWithEmptyGetRequest
    {
        public UnsignedByteParameterWrapper(in RDMParameterDescription parameterDescription) : base(parameterDescription)
        {
        }
        protected override byte getResponseParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] getResponseValueToParameterData(byte value)
        {
            return Tools.ValueToData(value);
        }

        protected override byte setRequestParameterDataToValue(byte[] parameterData)
        {
            return Tools.DataToByte(ref parameterData);
        }

        protected override byte[] setRequestValueToParameterData(byte value)
        {
            return Tools.ValueToData(value);
        }
    }
}