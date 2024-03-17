namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperResponse<TResponse> : IRDMGetParameterWrapperResponse
    {
        RDMMessage BuildGetResponseMessage(TResponse value);
        TResponse GetResponseParameterDataToValue(byte[] parameterData);
        byte[] GetResponseValueToParameterData(TResponse value);
    }
}