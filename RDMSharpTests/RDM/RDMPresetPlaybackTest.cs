namespace RDMSharpTest.RDM
{
    public class RDMPresetPlaybackTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMPresetPlayback presetPlayback = new RDMPresetPlayback(3333, 125);
            byte[] data = presetPlayback.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PRESET_PLAYBACK,
                ParameterData = data,
            };

            RDMPresetPlayback resultPresetPlayback = RDMPresetPlayback.FromMessage(message);

            Assert.That(resultPresetPlayback, Is.EqualTo(presetPlayback));

            var res = resultPresetPlayback.ToString();
            var src = presetPlayback.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}