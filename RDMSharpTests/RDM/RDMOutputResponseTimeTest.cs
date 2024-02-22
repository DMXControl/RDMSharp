namespace RDMSharpTest.RDM
{
    public class RDMOutputResponseTimeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMOutputResponseTime outputResponseTime = new RDMOutputResponseTime(1, 5);
            byte[] data = outputResponseTime.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.OUTPUT_RESPONSE_TIME,
                ParameterData = data,
            };

            RDMOutputResponseTime resultOutputResponseTime = RDMOutputResponseTime.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMOutputResponseTime.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultOutputResponseTime, Is.EqualTo(outputResponseTime));

            var res = resultOutputResponseTime.ToString();
            var src = outputResponseTime.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}