namespace RDMSharpTest.RDM
{
    public class RDMModulationFrequencyTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMModulationFrequency modulationFrequency = new RDMModulationFrequency(233, 234);
            byte[] data = modulationFrequency.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MODULATION_FREQUENCY,
                ParameterData = data,
            };

            RDMModulationFrequency resultModulationFrequency = RDMModulationFrequency.FromMessage(message);

            Assert.That(resultModulationFrequency, Is.EqualTo(modulationFrequency));

            var res = resultModulationFrequency.ToString();
            var src = modulationFrequency.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}