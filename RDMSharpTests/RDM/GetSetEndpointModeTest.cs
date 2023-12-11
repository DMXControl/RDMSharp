namespace RDMSharpTest.RDM
{
    public class GetSetEndpointModeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointMode getSetEndpointMode = new GetSetEndpointMode(1, ERDM_EndpointMode.OUTPUT);
            byte[] data = getSetEndpointMode.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_MODE,
                ParameterData = data,
            };

            GetSetEndpointMode resultGetSetEndpointMode = GetSetEndpointMode.FromMessage(message);

            Assert.That(resultGetSetEndpointMode, Is.EqualTo(getSetEndpointMode));
        }
    }
}