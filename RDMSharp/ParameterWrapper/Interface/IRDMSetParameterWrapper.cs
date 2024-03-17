namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapper<TRequest, TResponse> : IRDMSetParameterWrapperRequest<TRequest>, IRDMSetParameterWrapperResponse<TResponse>
    {
        ERDM_SupportedSubDevice SupportedSetSubDevices { get; }
    }
}