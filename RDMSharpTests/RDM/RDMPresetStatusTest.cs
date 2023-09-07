using RDMSharp;

namespace RDMSharpTest.RDM
{
    public class RDMPresetStatusTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMPresetStatus presetStatus = new RDMPresetStatus(12354, 21567, 7432, 23467, ERDM_PresetProgrammed.PROGRAMMED_READ_ONLY);
            byte[] data = presetStatus.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PRESET_STATUS,
                ParameterData = data,
            };

            RDMPresetStatus resultPresetStatus = RDMPresetStatus.FromMessage(message);

            Assert.AreEqual(presetStatus, resultPresetStatus);
        }
    }
}