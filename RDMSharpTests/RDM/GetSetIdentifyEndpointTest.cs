namespace RDMSharpTest.RDM
{
    public class GetSetIdentifyEndpointTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIdentifyEndpoint getSetIPv4StaticAddress = new GetSetIdentifyEndpoint(1, true);
            byte[] data = getSetIPv4StaticAddress.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IDENTIFY_ENDPOINT,
                ParameterData = data,
            };

            GetSetIdentifyEndpoint resultGetSetIPv4StaticAddress = GetSetIdentifyEndpoint.FromMessage(message);

            Assert.That(resultGetSetIPv4StaticAddress, Is.EqualTo(getSetIPv4StaticAddress));
        }
    }
}