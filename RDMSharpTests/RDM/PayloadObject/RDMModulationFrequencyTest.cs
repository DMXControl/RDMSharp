namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMModulationFrequency.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultModulationFrequency, Is.EqualTo(modulationFrequency));
            Assert.That(resultModulationFrequency.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(resultModulationFrequency.MinIndex, Is.EqualTo(1));
            Assert.That(resultModulationFrequency.Index, Is.EqualTo(233));
            Assert.That(resultModulationFrequency.Count, Is.EqualTo(234));
            Assert.That(resultModulationFrequency.DescriptorParameter, Is.EqualTo(ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION));

            var res = resultModulationFrequency.ToString();
            var src = modulationFrequency.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}