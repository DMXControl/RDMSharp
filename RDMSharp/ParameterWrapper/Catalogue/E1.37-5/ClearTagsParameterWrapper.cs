namespace RDMSharp.ParameterWrapper
{
    public sealed class ClearTagsParameterWrapper : AbstractRDMSetParameterWrapperEmptyRequestResponse
    {
        public ClearTagsParameterWrapper() : base(ERDM_Parameter.CLEAR_TAGS)
        {
        }
        public override string Name => "Clear Tags";
        public override string Description => "This parameter removes all tags from a Responder.";

    }
}