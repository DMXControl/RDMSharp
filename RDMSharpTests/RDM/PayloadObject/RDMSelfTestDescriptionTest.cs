namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMSelfTestDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMSelfTestDescription selfTestDescription = new RDMSelfTestDescription(1, "Pseudo Selftest");
            byte[] data = selfTestDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SELF_TEST_DESCRIPTION,
                ParameterData = data,
            };

            RDMSelfTestDescription resultSelfTestDescription = RDMSelfTestDescription.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMSelfTestDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultSelfTestDescription, Is.EqualTo(selfTestDescription));

            var res = resultSelfTestDescription.ToString();
            var src = selfTestDescription.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMSelfTestDescription parameterDescription = new RDMSelfTestDescription(description: "Pseudo Selftest 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(parameterDescription.Description, Has.Length.EqualTo(32));

            parameterDescription = new RDMSelfTestDescription(4, description: "");
            Assert.That(string.IsNullOrEmpty(parameterDescription.Description), Is.True);
        }
    }
}