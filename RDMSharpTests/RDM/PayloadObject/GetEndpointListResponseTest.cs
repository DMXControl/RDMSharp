namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetEndpointListResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointListResponse getEndpointListResponse = new GetEndpointListResponse(0x12345678,
                new EndpointDescriptor(),
                new EndpointDescriptor(1, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(1),
                new EndpointDescriptor(1, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(2),
                new EndpointDescriptor(2, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(2),
                new EndpointDescriptor(3, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(3),
                new EndpointDescriptor(3, ERDM_EndpointType.PHYSICAL));

            byte[] data = getEndpointListResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_LIST,
                ParameterData = data,
            };

            GetEndpointListResponse resultGetEndpointListResponse = GetEndpointListResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetEndpointListResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetEndpointListResponse.FromPayloadData(data.ToList().Concat(new byte[220]).ToArray()); });

            Assert.That(resultGetEndpointListResponse, Is.EqualTo(getEndpointListResponse));

            var res = resultGetEndpointListResponse.ToString();
            var src = getEndpointListResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}