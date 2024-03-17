namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperRequest<TRequest> : IRDMGetParameterWrapperRequest
    {
        ERDM_Parameter[] DescriptiveParameters { get; }
        RequestRange<TRequest> GetRequestRange(object value);
        RDMMessage BuildGetRequestMessage(TRequest value);
        byte[] GetRequestValueToParameterData(TRequest value);
        TRequest GetRequestParameterDataToValue(byte[] parameterData);
    }
}