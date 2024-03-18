namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetBrokerStatusResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetBrokerStatusResponse, Is.EqualTo(getBrokerStatusResponse));

            var res = resultGetBrokerStatusResponse.ToString();
            var src = getBrokerStatusResponse.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}