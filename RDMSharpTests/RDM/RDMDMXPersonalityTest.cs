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
            Assert.Throws(typeof(Exception), () => { RDMDMXPersonality.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultRdmDmxPersonality, Is.EqualTo(rdmDmxPersonality));
            Assert.That(resultRdmDmxPersonality.MinIndex, Is.EqualTo(1));
            Assert.That(resultRdmDmxPersonality.Index, Is.EqualTo(1));
            Assert.That(resultRdmDmxPersonality.DescriptorParameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION));
            Assert.That(resultRdmDmxPersonality.IndexType, Is.EqualTo(typeof(byte)));

            var res = resultRdmDmxPersonality.ToString();
            var src = rdmDmxPersonality.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}