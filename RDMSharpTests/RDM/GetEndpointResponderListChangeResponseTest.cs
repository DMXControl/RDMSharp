using RDMSharp;
using NUnit.Framework;

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

            Assert.AreEqual(getEndpointResponderListChangeResponse, resultGetEndpointResponderListChangeResponse);
        }
    }
}