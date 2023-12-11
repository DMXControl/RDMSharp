using RDMSharp;

namespace RDMSharpTest.RDM
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
            RDMModulationFrequencyDescription modulationFrequencyDescription = new RDMModulationFrequencyDescription(1,null,"Pseudo ModulationFrequency");
            byte[] data = modulationFrequencyDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MODULATION_FREQUENCY_DESCRIPTION,
                ParameterData = data,
            };

            RDMModulationFrequencyDescription resultModulationFrequencyDescription = RDMModulationFrequencyDescription.FromMessage(message);

            Assert.That(modulationFrequencyDescription, Is.EqualTo(resultModulationFrequencyDescription));

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

            Assert.That(modulationFrequencyDescription, Is.EqualTo(resultModulationFrequencyDescription));
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

            Assert.That(modulationFrequencyDescription, Is.EqualTo(resultModulationFrequencyDescription));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMModulationFrequencyDescription resultModulationFrequencyDescription = new RDMModulationFrequencyDescription(description: "Pseudo ModulationFrequency 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultModulationFrequencyDescription.Description.Length,Is.EqualTo(32));
        }
    }
}