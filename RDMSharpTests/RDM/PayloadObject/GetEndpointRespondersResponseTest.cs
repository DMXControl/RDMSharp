namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetEndpointRespondersResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointRespondersResponse getEndpointRespondersResponse = new GetEndpointRespondersResponse(0x12345678,
                new RDMUID(123, 34567872),
                new RDMUID(654, 26323133),
                new RDMUID(932, 14567542),
                new RDMUID(923, 79812414),
                new RDMUID(124, 29836472),
                new RDMUID(986, 79832421));

            byte[] data = getEndpointRespondersResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_RESPONDERS,
                ParameterData = data,
            };

            GetEndpointRespondersResponse resultGetEndpointRespondersResponse = GetEndpointRespondersResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetEndpointRespondersResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetEndpointRespondersResponse.FromPayloadData(data.ToList().Concat(new byte[220]).ToArray()); });

            Assert.That(resultGetEndpointRespondersResponse, Is.EqualTo(getEndpointRespondersResponse));

            var res = resultGetEndpointRespondersResponse.ToString();
            var src = getEndpointRespondersResponse.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}