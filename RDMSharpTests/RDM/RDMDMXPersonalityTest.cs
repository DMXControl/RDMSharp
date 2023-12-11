namespace RDMSharpTest.RDM
{
    public class RDMDMXPersonalityTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDMXPersonality rdmDmxPersonality = new RDMDMXPersonality(1, 5);
            byte[] data = rdmDmxPersonality.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DMX_PERSONALITY,
                ParameterData = data,
            };

            RDMDMXPersonality resultRdmDmxPersonality = RDMDMXPersonality.FromMessage(message);

            Assert.That(resultRdmDmxPersonality, Is.EqualTo(rdmDmxPersonality));
        }
    }
}