using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetBackgroundQueuedStatusPolicyDescriptionResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetBackgroundQueuedStatusPolicyDescriptionResponse getBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(1, "Pseudo Background Queued/Status Message Policy Description");
            byte[] data = getBackgroundQueuedStatusPolicyDescriptionResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION,
                ParameterData = data,
            };

            GetBackgroundQueuedStatusPolicyDescriptionResponse resultGetBackgroundQueuedStatusPolicyDescriptionResponse = GetBackgroundQueuedStatusPolicyDescriptionResponse.FromMessage(message);

            Assert.AreEqual(getBackgroundQueuedStatusPolicyDescriptionResponse, resultGetBackgroundQueuedStatusPolicyDescriptionResponse);
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetEndpointTimingDescriptionResponse resultGetEndpointTimingDescriptionResponse = new GetEndpointTimingDescriptionResponse(description: "Pseudo Background Queued/Status Message Policy Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.AreEqual(32, resultGetEndpointTimingDescriptionResponse.Description.Length);
        }
    }
}