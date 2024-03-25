namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperGetResponseContravariance<in TResponse> : IRDMGetParameterWrapperResponse
    {
        RDMMessage BuildGetResponseMessage(TResponse value);
        byte[] GetResponseValueToParameterData(TResponse value);
    }
}