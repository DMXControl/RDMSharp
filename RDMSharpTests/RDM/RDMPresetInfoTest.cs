using RDMSharp;

namespace RDMSharpTest.RDM
{
    public class RDMPresetInfoTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMPresetInfo presetInfo = new RDMPresetInfo(true, true, true, true, true, true, 12354, 21567, 7432, 23467, 7632, 24567, 7532, 23456, ushort.MaxValue, 23456, 6543, ushort.MaxValue, 5432);
            byte[] data = presetInfo.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PRESET_INFO,
                ParameterData = data,
            };

            RDMPresetInfo resultPresetInfo = RDMPresetInfo.FromMessage(message);

            Assert.That(presetInfo, Is.EqualTo(resultPresetInfo));
        }
    }
}