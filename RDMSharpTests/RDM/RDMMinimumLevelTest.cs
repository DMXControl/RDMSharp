namespace RDMSharpTest.RDM
{
    public class RDMMinimumLevelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMMinimumLevel minimumLevel = new RDMMinimumLevel(10303, 43211, true);
            byte[] data = minimumLevel.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.MINIMUM_LEVEL,
                ParameterData = data,
            };

            RDMMinimumLevel resultMinimumLevel = RDMMinimumLevel.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMMinimumLevel.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultMinimumLevel, Is.EqualTo(minimumLevel));

            var res = resultMinimumLevel.ToString();
            var src = minimumLevel.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}