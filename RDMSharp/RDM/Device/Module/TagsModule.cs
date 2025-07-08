namespace RDMSharp.RDM.Device.Module
{
    public sealed class TagsModule : AbstractModule
    {
        public TagsModule() : base(
            "Tags",
            ERDM_Parameter.LIST_TAGS,
            ERDM_Parameter.ADD_TAG,
            ERDM_Parameter.REMOVE_TAG,
            ERDM_Parameter.CHECK_TAG,
            ERDM_Parameter.CLEAR_TAGS)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            throw new System.NotImplementedException();
        }

        //protected override RDMMessage handleRequest(RDMMessage message)
        //{
        //    return null;
        //}
    }
}