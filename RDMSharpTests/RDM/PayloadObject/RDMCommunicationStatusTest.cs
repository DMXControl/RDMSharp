namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMCommunicationStatusTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMCommunicationStatus communicationStatus = new RDMCommunicationStatus(55, 42, 11880);

            byte[] data = communicationStatus.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMMS_STATUS,
                ParameterData = data,
            };

            RDMCommunicationStatus resultCommunicationStatus = RDMCommunicationStatus.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMCommunicationStatus.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultCommunicationStatus, Is.EqualTo(communicationStatus));

            var res = resultCommunicationStatus.ToString();
            var src = communicationStatus.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}