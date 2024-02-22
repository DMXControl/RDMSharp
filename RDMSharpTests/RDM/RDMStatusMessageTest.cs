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

            var res = resultStatusMessage.ToString();
            var src = statusMessage.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}