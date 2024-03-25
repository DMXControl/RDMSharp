namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequest<TRequest> : IRDMGetParameterWrapperRequest, IRDMGetParameterWrapperRequestContravariance<TRequest>
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
        TRequest GetRequestParameterDataToValue(byte[] parameterData);
        new IRequestRange<TRequest> GetRequestRange(object value);
    }
}