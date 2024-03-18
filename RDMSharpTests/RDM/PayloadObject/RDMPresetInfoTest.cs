namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMPresetInfo.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultPresetInfo, Is.EqualTo(presetInfo));

            var res = resultPresetInfo.ToString();
            var src = presetInfo.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}