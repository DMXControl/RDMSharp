namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperSetResponseContravariance<in TResponse> : IRDMSetParameterWrapperResponse
    {
        RDMMessage BuildSetResponseMessage(TResponse value);
        byte[] SetResponseValueToParameterData(TResponse value);
    }
}