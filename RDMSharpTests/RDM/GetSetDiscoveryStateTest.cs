using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetSetDiscoveryStateTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetDiscoveryStateResponse getDiscoveryStateResponse = new GetDiscoveryStateResponse(1, 123, ERDM_DiscoveryState.FULL);
            byte[] data = getDiscoveryStateResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISCOVERY_STATE,
                ParameterData = data,
            };

            GetDiscoveryStateResponse resultGetDiscoveryStateResponse = GetDiscoveryStateResponse.FromMessage(message);

            Assert.AreEqual(getDiscoveryStateResponse, resultGetDiscoveryStateResponse);

            SetDiscoveryStateRequest setDiscoveryStateRequest = new SetDiscoveryStateRequest(1, ERDM_DiscoveryState.INCREMENTAL);
            data = setDiscoveryStateRequest.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISCOVERY_STATE,
                ParameterData = data,
            };

            SetDiscoveryStateRequest resultSetDiscoveryStateRequest = SetDiscoveryStateRequest.FromMessage(message);

            Assert.AreEqual(setDiscoveryStateRequest, resultSetDiscoveryStateRequest);
        }
    }
}