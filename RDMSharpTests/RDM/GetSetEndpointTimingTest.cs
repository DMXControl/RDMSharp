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

            Assert.That(resultGetEndpointTimingResponse, Is.EqualTo(getEndpointTimingResponse));

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

            Assert.That(resultSetEndpointTimingRequest, Is.EqualTo(setEndpointTimingRequest));
        }
    }
}