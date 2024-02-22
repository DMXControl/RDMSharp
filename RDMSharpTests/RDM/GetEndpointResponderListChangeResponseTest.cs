namespace RDMSharpTest.RDM
{
    public class GetEndpointResponderListChangeResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointResponderListChangeResponse getEndpointResponderListChangeResponse = new GetEndpointResponderListChangeResponse(1, 1234);
            byte[] data = getEndpointResponderListChangeResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_RESPONDER_LIST_CHANGE,
                ParameterData = data,
            };

            GetEndpointResponderListChangeResponse resultGetEndpointResponderListChangeResponse = GetEndpointResponderListChangeResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetEndpointResponderListChangeResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetEndpointResponderListChangeResponse, Is.EqualTo(getEndpointResponderListChangeResponse));

            var res = resultGetEndpointResponderListChangeResponse.ToString();
            var src = getEndpointResponderListChangeResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}