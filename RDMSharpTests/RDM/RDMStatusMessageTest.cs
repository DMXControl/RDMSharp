using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class RDMStatusMessageTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMStatusMessage statusMessage = new RDMStatusMessage(0, ERDM_Status.ERROR, ERDM_StatusMessage.UNDERCURRENT, 2, 20);

            byte[] data = statusMessage.ToPayloadData();

            RDMStatusMessage resultStatusMessage = RDMStatusMessage.FromPayloadData(data);

            Assert.That(resultStatusMessage, Is.EqualTo(statusMessage));
        }
    }
}