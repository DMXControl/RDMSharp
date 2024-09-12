namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequest<TRequest> : IRDMGetParameterWrapperRequest, IRDMGetParameterWrapperRequestContravariance<TRequest>
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
        TRequest GetRequestParameterDataToValue(byte[] parameterData);
    }
    public interface IRDMGetParameterWrapperRequestRanged<TRequest> : IRDMGetParameterWrapperRequest<TRequest>
    {
        new IRequestRange<TRequest> GetRequestRange(object value);
    }
}