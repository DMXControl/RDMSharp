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
            Assert.Throws(typeof(Exception), () => { RDMDMXPersonalityDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultRdmDmxPersonalityDescription, Is.EqualTo(rdmDmxPersonalityDescription));

            var res = resultRdmDmxPersonalityDescription.ToString();
            var src = rdmDmxPersonalityDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMDMXPersonalityDescription rdmDmxPersonalityDescription = new RDMDMXPersonalityDescription(description: "Pseudo Personality 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(rdmDmxPersonalityDescription.Description.Length, Is.EqualTo(32));
            rdmDmxPersonalityDescription = new RDMDMXPersonalityDescription(description: "");
            Assert.That(string.IsNullOrWhiteSpace(rdmDmxPersonalityDescription.Description), Is.True);
            Assert.That(rdmDmxPersonalityDescription.MinIndex, Is.EqualTo(1));
        }
    }
}