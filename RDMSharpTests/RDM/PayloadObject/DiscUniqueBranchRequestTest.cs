namespace RDMSharpTests.RDM.PayloadObject
{
    public class DiscUniqueBranchRequestTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            DiscUniqueBranchRequest discUniqueBranch = new DiscUniqueBranchRequest(new RDMUID(223, 434), new RDMUID(3333, 99999));

            byte[] data = discUniqueBranch.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                ParameterData = data,
            };

            DiscUniqueBranchRequest resultDiscMute = DiscUniqueBranchRequest.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { DiscUniqueBranchRequest.FromPayloadData(data.ToList().Concat(new byte[2]).ToArray()); });

            Assert.That(resultDiscMute, Is.EqualTo(discUniqueBranch));

            var res = resultDiscMute.ToString();
            var src = discUniqueBranch.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}