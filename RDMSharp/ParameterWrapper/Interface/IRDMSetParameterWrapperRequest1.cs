namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperRequest<TRequest> : IRDMSetParameterWrapperRequestContravariance<TRequest>
    {
        TRequest SetRequestParameterDataToValue(byte[] parameterData);
    }
}