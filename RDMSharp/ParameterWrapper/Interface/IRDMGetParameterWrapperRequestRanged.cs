namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequestRanged<TRequest> : IRDMGetParameterWrapperRequest<TRequest>
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
        new IRequestRange<TRequest> GetRequestRange(object value);
    }
}