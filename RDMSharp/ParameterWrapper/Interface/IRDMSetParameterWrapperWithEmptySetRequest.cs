namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperWithEmptySetRequest : IRDMParameterWrapper
    {
        RDMMessage BuildSetRequestMessage();
    }
}