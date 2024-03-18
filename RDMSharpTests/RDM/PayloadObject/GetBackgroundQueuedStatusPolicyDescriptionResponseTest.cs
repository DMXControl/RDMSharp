namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetBackgroundQueuedStatusPolicyDescriptionResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse, Is.EqualTo(getBackgroundQueuedStatusPolicyDescriptionResponse));

            var res = resultGetBackgroundQueuedStatusPolicyDescriptionResponse.ToString();
            var src = getBackgroundQueuedStatusPolicyDescriptionResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetBackgroundQueuedStatusPolicyDescriptionResponse resultGetBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(description: "Pseudo Background Queued/Status Message Policy Description 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Description.Length, Is.EqualTo(32));

            resultGetBackgroundQueuedStatusPolicyDescriptionResponse = new GetBackgroundQueuedStatusPolicyDescriptionResponse(5, description: "");
            Assert.That(string.IsNullOrEmpty(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Description), Is.True);
            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.MinIndex, Is.EqualTo(1));
            Assert.That(resultGetBackgroundQueuedStatusPolicyDescriptionResponse.Index, Is.EqualTo(5));
        }
    }
}