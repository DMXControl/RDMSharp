namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequestContravariance<in TRequest> : IRDMGetParameterWrapperRequest
    {
        RDMMessage BuildGetRequestMessage(TRequest value);
        byte[] GetRequestValueToParameterData(TRequest value);
    }
}