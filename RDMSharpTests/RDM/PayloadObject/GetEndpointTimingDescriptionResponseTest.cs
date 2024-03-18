namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetEndpointTimingDescriptionResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetEndpointTimingDescriptionResponse, Is.EqualTo(getGetEndpointTimingDescriptionResponse));

            var res = resultGetEndpointTimingDescriptionResponse.ToString();
            var src = getGetEndpointTimingDescriptionResponse.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(description: "Pseudo Endpoint Timing Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.Multiple(() =>
            {
                Assert.That(resultGetEndpointTimingDescriptionResponse.Description, Has.Length.EqualTo(32));
                Assert.That(resultGetEndpointTimingDescriptionResponse.MinIndex, Is.EqualTo(1));
                Assert.That(resultGetEndpointTimingDescriptionResponse.Index, Is.EqualTo(1));
            });

            resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(10, description: "");
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrWhiteSpace(resultGetEndpointTimingDescriptionResponse.Description), Is.True);
                Assert.That(resultGetEndpointTimingDescriptionResponse.MinIndex, Is.EqualTo(1));
                Assert.That(resultGetEndpointTimingDescriptionResponse.Index, Is.EqualTo(10));
            });
        }
    }
}