namespace RDMSharp.ParameterWrapper
{
    public sealed class CapturePresetParameterWrapper : AbstractRDMSetParameterWrapperEmptyResponse<RDMPresetStatus>
    {
        public CapturePresetParameterWrapper() : base(ERDM_Parameter.CAPTURE_PRESET)
        {
        }
        public override string Name => "Capture Preset";
        public override string Description => "This parameter is used to capture a static scene into a Preset within the responder. " +
            "Upon receipt of this parameter the responder will capture the scene and store it to the designated preset.\r\n" +
            "Fade and Wait times for building sequences may also be included. Times are in tenths of a " +
            "second. When timing information is not required it is set to 0x00.\r\n" +
            "The Up Fade Time is the fade in time for the current scene and the Down Fade Time is the down " +
            "fade for the previous scene or active look. The Wait Time is the time the device spends holding" +
            "the current scene before proceeding to play the next scene when the presets are being played " +
            "back as a sequence.";

        protected override RDMPresetStatus setRequestParameterDataToValue(byte[] parameterData)
        {
            return RDMPresetStatus.FromPayloadData(parameterData);
        }

        protected override byte[] setRequestValueToParameterData(RDMPresetStatus capturePreset)
        {
            return capturePreset.ToPayloadData();
        }
    }
}