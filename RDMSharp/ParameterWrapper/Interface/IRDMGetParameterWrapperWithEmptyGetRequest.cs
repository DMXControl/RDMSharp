namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperWithEmptyGetRequest : IRDMParameterWrapper
    {
        RDMMessage BuildGetRequestMessage();
    }
}