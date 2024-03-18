namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMModulationFrequencyDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTestWithoutFrequency()
        {
            RDMModulationFrequencyDescription modulationFrequencyDescription = new RDMModulationFrequencyDescription(1, null, "Pseudo ModulationFrequency");
            byte[] data = modulationFrequencyDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
                ParameterData = data,
            };

            RDMModulationFrequencyDescription resultModulationFrequencyDescription = RDMModulationFrequencyDescription.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMModulationFrequencyDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultModulationFrequencyDescription, Is.EqualTo(modulationFrequencyDescription));

            var res = resultModulationFrequencyDescription.ToString();
            var src = modulationFrequencyDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            modulationFrequencyDescription = new RDMModulationFrequencyDescription(1, uint.MaxValue, "Pseudo ModulationFrequency");
            data = modulationFrequencyDescription.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
                ParameterData = data,
            };

            resultModulationFrequencyDescription = RDMModulationFrequencyDescription.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMModulationFrequencyDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultModulationFrequencyDescription, Is.EqualTo(modulationFrequencyDescription));

            res = resultModulationFrequencyDescription.ToString();
            src = modulationFrequencyDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMModulationFrequencyDescription modulationFrequencyDescription = new RDMModulationFrequencyDescription(1, 3456543131, "Pseudo ModulationFrequency");
            byte[] data = modulationFrequencyDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
                ParameterData = data,
            };

            RDMModulationFrequencyDescription resultModulationFrequencyDescription = RDMModulationFrequencyDescription.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMModulationFrequencyDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultModulationFrequencyDescription, Is.EqualTo(modulationFrequencyDescription));

            var res = resultModulationFrequencyDescription.ToString();
            var src = modulationFrequencyDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMModulationFrequencyDescription resultModulationFrequencyDescription = new RDMModulationFrequencyDescription(description: "Pseudo ModulationFrequency 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultModulationFrequencyDescription.Description.Length, Is.EqualTo(32));

            resultModulationFrequencyDescription = new RDMModulationFrequencyDescription(5, description: "");
            Assert.That(string.IsNullOrEmpty(resultModulationFrequencyDescription.Description), Is.True);
            Assert.That(resultModulationFrequencyDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultModulationFrequencyDescription.Index, Is.EqualTo(5));
        }
    }
}