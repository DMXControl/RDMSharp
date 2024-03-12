namespace RDMSharpTests.RDM.PayloadObject
{
    public class DiscMuteUnmuteResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            DiscMuteUnmuteResponse discMute = new DiscMuteUnmuteResponse(true, bindingUID: new RDMUID(223, 434));

            byte[] data = discMute.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISC_MUTE,
                ParameterData = data,
            };

            DiscMuteUnmuteResponse resultDiscMute = DiscMuteUnmuteResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { DiscMuteUnmuteResponse.FromPayloadData(data.ToList().Concat(new byte[2]).ToArray()); });

            Assert.That(resultDiscMute, Is.EqualTo(discMute));

            var res = resultDiscMute.ToString();
            var src = discMute.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            discMute = new DiscMuteUnmuteResponse(false, true, bindingUID: new RDMUID(223, 434));

            data = discMute.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
                ParameterData = data,
            };

            resultDiscMute = DiscMuteUnmuteResponse.FromMessage(message);

            Assert.That(resultDiscMute, Is.EqualTo(discMute));

            res = resultDiscMute.ToString();
            src = discMute.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            discMute = new DiscMuteUnmuteResponse(false, false, true, bindingUID: new RDMUID(223, 434));

            data = discMute.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
                ParameterData = data,
            };

            resultDiscMute = DiscMuteUnmuteResponse.FromMessage(message);

            Assert.That(resultDiscMute, Is.EqualTo(discMute));

            res = resultDiscMute.ToString();
            src = discMute.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            discMute = new DiscMuteUnmuteResponse(false, false, false, true);

            data = discMute.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
                ParameterData = data,
            };

            resultDiscMute = DiscMuteUnmuteResponse.FromMessage(message);

            Assert.That(resultDiscMute, Is.EqualTo(discMute));

            res = resultDiscMute.ToString();
            src = discMute.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}