namespace RDMSharp.ParameterWrapper
{
    public interface IRDMSetParameterWrapperWithEmptySetResponse : IRDMParameterWrapper
    {
        RDMMessage BuildSetResponseMessage();
    }
}