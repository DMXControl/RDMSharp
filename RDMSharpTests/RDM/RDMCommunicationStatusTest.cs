using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
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
            RDMCommunicationStatus communicationStatus = new RDMCommunicationStatus(55,42,11880);

            byte[] data = communicationStatus.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMMS_STATUS,
                ParameterData = data,
            };

            RDMCommunicationStatus resultCommunicationStatus = RDMCommunicationStatus.FromMessage(message);

            Assert.That(resultCommunicationStatus, Is.EqualTo(communicationStatus));
        }
    }
}