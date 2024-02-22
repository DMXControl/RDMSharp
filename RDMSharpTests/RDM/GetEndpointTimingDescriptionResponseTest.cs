namespace RDMSharpTest.RDM
{
    public class GetEndpointTimingDescriptionResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointTimingDescriptionResponse getGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(1, "Pseudo Endpoint Timing Description");
            byte[] data = getGetEndpointTimingDescriptionResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_TIMING_DESCRIPTION,
                ParameterData = data,
            };

            GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = GetEndpointTimingDescriptionResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetEndpointTimingDescriptionResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetEndpointTimingDescriptionResponse, Is.EqualTo(getGetEndpointTimingDescriptionResponse));

            var res = resultGetEndpointTimingDescriptionResponse.ToString();
            var src = getGetEndpointTimingDescriptionResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(description: "Pseudo Endpoint Timing Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultGetEndpointTimingDescriptionResponse.Description.Length, Is.EqualTo(32));
        }
    }
}