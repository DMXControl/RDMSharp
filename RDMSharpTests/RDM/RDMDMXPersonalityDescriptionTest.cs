namespace RDMSharpTest.RDM
{
    public class RDMDMXPersonalityDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDMXPersonalityDescription rdmDmxPersonalityDescription = new RDMDMXPersonalityDescription(42, 5, "Pseudo Personality");
            byte[] data = rdmDmxPersonalityDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
                ParameterData = data,
            };

            RDMDMXPersonalityDescription resultRdmDmxPersonalityDescription = RDMDMXPersonalityDescription.FromMessage(message);

            Assert.That(rdmDmxPersonalityDescription, Is.EqualTo(resultRdmDmxPersonalityDescription));
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMDMXPersonalityDescription rdmDmxPersonalityDescription = new RDMDMXPersonalityDescription(description: "Pseudo Personality 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(rdmDmxPersonalityDescription.Description.Length, Is.EqualTo(32));
        }
    }
}