namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetDiscoveryStateResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetDiscoveryStateResponse, Is.EqualTo(getDiscoveryStateResponse));

            var res = resultGetDiscoveryStateResponse.ToString();
            var src = getDiscoveryStateResponse.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));

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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { SetDiscoveryStateRequest.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSetDiscoveryStateRequest, Is.EqualTo(setDiscoveryStateRequest));

            res = resultSetDiscoveryStateRequest.ToString();
            src = setDiscoveryStateRequest.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}