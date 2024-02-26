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
            RDMOutputResponseTime outputResponseTime = new RDMOutputResponseTime(14, 5);
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
            Assert.That(resultOutputResponseTime.IndexType, Is.EqualTo(typeof(byte)));
            Assert.That(resultOutputResponseTime.MinIndex, Is.EqualTo(1));
            Assert.That(resultOutputResponseTime.Index, Is.EqualTo(14));
            Assert.That(resultOutputResponseTime.Count, Is.EqualTo(5));
            Assert.That(resultOutputResponseTime.DescriptorParameter, Is.EqualTo(ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION));

            var res = resultOutputResponseTime.ToString();
            var src = outputResponseTime.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}