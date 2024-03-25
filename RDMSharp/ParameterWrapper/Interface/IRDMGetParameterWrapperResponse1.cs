namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperResponse<TResponse> : IRDMGetParameterWrapperResponse, IRDMGetParameterWrapperGetResponseContravariance<TResponse>
    {
        TResponse GetResponseParameterDataToValue(byte[] parameterData);
    }
}