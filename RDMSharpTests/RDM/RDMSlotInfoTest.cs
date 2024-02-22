namespace RDMSharpTest.RDM
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

            RDMSlotInfo resultSlotInfo = RDMSlotInfo.FromPayloadData(data);

            Assert.That(resultSlotInfo, Is.EqualTo(slotInfo));

            var res = resultSlotInfo.ToString();
            var src = slotInfo.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}