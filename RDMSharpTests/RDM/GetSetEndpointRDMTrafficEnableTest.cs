namespace RDMSharpTest.RDM
{
    public class GetSetEndpointRDMTrafficEnableTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointRDMTrafficEnable getSetEndpointRDMTrafficEnabled = new GetSetEndpointRDMTrafficEnable(1, true);
            byte[] data = getSetEndpointRDMTrafficEnabled.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.RDM_TRAFFIC_ENABLE,
                ParameterData = data,
            };

            GetSetEndpointRDMTrafficEnable resultGetSetEndpointRDMTrafficEnable = GetSetEndpointRDMTrafficEnable.FromMessage(message);

            Assert.That(resultGetSetEndpointRDMTrafficEnable, Is.EqualTo(getSetEndpointRDMTrafficEnabled));
        }
    }
}