namespace RDMSharpTest.RDM
{
    public class GetBackgroundQueuedStatusPolicyResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetBackgroundQueuedStatusPolicyResponse getBackgroundQueuedStatusPolicyResponse = new GetBackgroundQueuedStatusPolicyResponse(1, 123);
            byte[] data = getBackgroundQueuedStatusPolicyResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY,
                ParameterData = data,
            };

            GetBackgroundQueuedStatusPolicyResponse resultGetBackgroundQueuedStatusPolicyResponse = GetBackgroundQueuedStatusPolicyResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetBackgroundQueuedStatusPolicyResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse, Is.EqualTo(getBackgroundQueuedStatusPolicyResponse));

            var res = resultGetBackgroundQueuedStatusPolicyResponse.ToString();
            var src = getBackgroundQueuedStatusPolicyResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}