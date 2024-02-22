namespace RDMSharpTest.RDM
{
    public class RDMCurveTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMCurve curve = new RDMCurve(1, 5);
            byte[] data = curve.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.CURVE,
                ParameterData = data,
            };

            RDMCurve resultCurve = RDMCurve.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMCurve.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultCurve, Is.EqualTo(curve));

            var res = resultCurve.ToString();
            var src = curve.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}