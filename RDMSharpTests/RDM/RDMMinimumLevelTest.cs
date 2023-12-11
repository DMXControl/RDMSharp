namespace RDMSharpTest.RDM
{
    public class RDMMinimumLevelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMMinimumLevel minimumLevel = new RDMMinimumLevel(10303, 43211, true);
            byte[] data = minimumLevel.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MINIMUM_LEVEL,
                ParameterData = data,
            };

            RDMMinimumLevel resultMinimumLevel = RDMMinimumLevel.FromMessage(message);

            Assert.That(minimumLevel, Is.EqualTo(resultMinimumLevel));
        }
    }
}