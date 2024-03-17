namespace RDMSharp.ParameterWrapper
{
    public interface IRDMGetParameterWrapperWithEmptyGetResponse : IRDMParameterWrapper
    {
        RDMMessage BuildGetResponseMessage();
    }
}