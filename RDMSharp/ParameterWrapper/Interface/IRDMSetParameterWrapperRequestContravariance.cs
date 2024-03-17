namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperRequestContravariance<in TRequest> : IRDMSetParameterWrapperRequest
    {
        RDMMessage BuildSetRequestMessage(TRequest value);
        byte[] SetRequestValueToParameterData(TRequest value);
    }
}