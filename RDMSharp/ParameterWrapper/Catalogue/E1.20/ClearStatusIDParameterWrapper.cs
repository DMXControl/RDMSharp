namespace RDMSharp.ParameterWrapper
{
    public sealed class ClearStatusIDParameterWrapper : AbstractRDMSetParameterWrapperEmptyRequestResponse
    {
        public ClearStatusIDParameterWrapper() : base(ERDM_Parameter.CLEAR_STATUS_ID)
        {
        }
        public override string Name => "Clear Status ID";
        public override string Description => "This parameter is used to clear the status message queue.";
    }
}