namespace RDMSharpTest.RDM
{
    public class GetBrokerStatusResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetBrokerStatusResponse getBrokerStatusResponse = new GetBrokerStatusResponse(true, ERDM_BrokerStatus.STANDBY);
            byte[] data = getBrokerStatusResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BROKER_STATUS,
                ParameterData = data,
            };

            GetBrokerStatusResponse resultGetBrokerStatusResponse = GetBrokerStatusResponse.FromMessage(message);

            Assert.That(resultGetBrokerStatusResponse, Is.EqualTo(getBrokerStatusResponse));
        }
    }
}