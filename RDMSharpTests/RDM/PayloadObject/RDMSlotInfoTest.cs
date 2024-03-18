namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMSlotInfoTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMSlotInfo slotInfo = new RDMSlotInfo(3, ERDM_SlotType.SEC_ROTATION, ERDM_SlotCategory.INTENSITY);

            byte[] data = slotInfo.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SLOT_INFO,
                ParameterData = data,
            };

            RDMSlotInfo resultSlotInfo = RDMSlotInfo.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMSlotInfo.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSlotInfo, Is.EqualTo(slotInfo));

            var res = resultSlotInfo.ToString();
            var src = slotInfo.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}