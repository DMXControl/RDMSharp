using RDMSharp;
using NUnit.Framework;

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
            RDMSlotInfo statusMessage = new RDMSlotInfo(3, ERDM_SlotType.SEC_ROTATION, ERDM_SlotCategory.INTENSITY);

            byte[] data = statusMessage.ToPayloadData();

            RDMSlotInfo resultStatusMessage = RDMSlotInfo.FromPayloadData(data);

            Assert.That(resultStatusMessage, Is.EqualTo(statusMessage));
        }
    }
}