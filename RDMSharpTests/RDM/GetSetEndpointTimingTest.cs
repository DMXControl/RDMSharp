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
            Assert.That(resultGetEndpointTimingResponse.EndpointId, Is.EqualTo(1));
            Assert.That(resultGetEndpointTimingResponse.TimingId, Is.EqualTo(123));
            Assert.That(resultGetEndpointTimingResponse.Timings, Is.EqualTo(254));

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


            getEndpointTimingResponse = new GetEndpointTimingResponse(31, 123, 254);
            Assert.That(getEndpointTimingResponse.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(getEndpointTimingResponse.MinIndex, Is.EqualTo(1));
            Assert.That(getEndpointTimingResponse.EndpointId, Is.EqualTo(31));
            Assert.That(getEndpointTimingResponse.Index, Is.EqualTo(123));
            Assert.That(getEndpointTimingResponse.Count, Is.EqualTo(254));
            Assert.That(getEndpointTimingResponse.DescriptorParameter, Is.EqualTo(ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION));
        }
    }
}