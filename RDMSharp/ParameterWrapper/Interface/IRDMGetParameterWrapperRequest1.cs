namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequest<TRequest> : IRDMGetParameterWrapperRequest, IRDMGetParameterWrapperRequestContravariance<TRequest>
    {
        TRequest GetRequestParameterDataToValue(byte[] parameterData);
    }
}