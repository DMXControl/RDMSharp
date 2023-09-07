using RDMSharp;
using NUnit.Framework;

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

            Assert.AreEqual(getGetEndpointTimingDescriptionResponse, resultGetEndpointTimingDescriptionResponse);
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(description: "Pseudo Endpoint Timing Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.AreEqual(32, resultGetEndpointTimingDescriptionResponse.Description.Length);
        }
    }
}