namespace RDMSharp.RDM.Device.Module
{
    public sealed class SelfTestsModule : AbstractModule
    {
        public SelfTestsModule() : base(
            "SelfTests",
            ERDM_Parameter.PERFORM_SELFTEST,
            ERDM_Parameter.SELF_TEST_DESCRIPTION)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            throw new System.NotImplementedException();
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            throw new System.NotImplementedException();
        }

        //protected override RDMMessage handleRequest(RDMMessage message)
        //{
        //    return null;
        //}
    }
}