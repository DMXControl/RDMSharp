namespace RDMSharpTest.RDM
{
    public class RDMStatusMessageTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMStatusMessage statusMessage = new RDMStatusMessage(0, ERDM_Status.ERROR, ERDM_StatusMessage.UNDERCURRENT, 2, 20);

            byte[] data = statusMessage.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.STATUS_MESSAGES,
                ParameterData = data,
            };

            RDMStatusMessage resultStatusMessage = RDMStatusMessage.FromMessage(message);

            Assert.That(resultStatusMessage, Is.EqualTo(statusMessage));

            var res = resultStatusMessage.ToString();
            var src = statusMessage.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}