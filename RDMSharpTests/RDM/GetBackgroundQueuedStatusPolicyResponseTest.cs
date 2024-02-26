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
            GetBackgroundQueuedStatusPolicyResponse getBackgroundQueuedStatusPolicyResponse = new GetBackgroundQueuedStatusPolicyResponse(9, 123);
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
            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse.MinIndex, Is.EqualTo(1));
            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse.Index, Is.EqualTo(9));
            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse.Count, Is.EqualTo(123));
            Assert.That(resultGetBackgroundQueuedStatusPolicyResponse.DescriptorParameter, Is.EqualTo(ERDM_Parameter.BACKGROUND_QUEUED_STATUS_POLICY_DESCRIPTION));

            var res = resultGetBackgroundQueuedStatusPolicyResponse.ToString();
            var src = getBackgroundQueuedStatusPolicyResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}