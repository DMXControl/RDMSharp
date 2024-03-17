namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperResponse<TResponse> : IRDMSetParameterWrapperSetResponseContravariance<TResponse>
    {
        TResponse SetResponseParameterDataToValue(byte[] parameterData);
    }
}