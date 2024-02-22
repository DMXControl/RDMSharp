namespace RDMSharpTest.RDM
{
    public class GetSetEndpointTimingTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointTimingResponse getEndpointTimingResponse = new GetEndpointTimingResponse(1, 123, 254);
            byte[] data = getEndpointTimingResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_TIMING,
                ParameterData = data,
            };

            GetEndpointTimingResponse resultGetEndpointTimingResponse = GetEndpointTimingResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetEndpointTimingResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetEndpointTimingResponse, Is.EqualTo(getEndpointTimingResponse));

            var res = resultGetEndpointTimingResponse.ToString();
            var src = getEndpointTimingResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            SetEndpointTimingRequest setEndpointTimingRequest = new SetEndpointTimingRequest(1, 42);
            data = setEndpointTimingRequest.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_TIMING,
                ParameterData = data,
            };

            SetEndpointTimingRequest resultSetEndpointTimingRequest = SetEndpointTimingRequest.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { SetEndpointTimingRequest.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSetEndpointTimingRequest, Is.EqualTo(setEndpointTimingRequest));

            res = resultSetEndpointTimingRequest.ToString();
            src = setEndpointTimingRequest.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}