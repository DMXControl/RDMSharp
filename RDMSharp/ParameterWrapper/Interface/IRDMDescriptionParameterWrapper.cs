namespace RDMSharp.ParameterWrapper
{
    public interface IRDMDescriptionParameterWrapper : IRDMParameterWrapper, IRDMGetParameterWrapperRequest, IRDMGetParameterWrapperResponse
    {
        ERDM_Parameter ValueParameterID { get; }
    }
}