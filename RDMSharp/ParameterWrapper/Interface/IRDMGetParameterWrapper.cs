namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapper<TRequest, TResponse> : IRDMGetParameterWrapperRequest<TRequest>, IRDMGetParameterWrapperResponse<TResponse>
    {
        ERDM_SupportedSubDevice SupportedGetSubDevices { get; }
    }
}