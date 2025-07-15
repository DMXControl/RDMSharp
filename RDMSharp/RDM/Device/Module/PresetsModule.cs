namespace RDMSharp.RDM.Device.Module
{
    public sealed class PresetsModule : AbstractModule
    {
        public PresetsModule() : base(
            "Presets",
            ERDM_Parameter.PRESET_PLAYBACK,
            ERDM_Parameter.CAPTURE_PRESET)
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