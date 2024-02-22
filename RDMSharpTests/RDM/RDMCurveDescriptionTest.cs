namespace RDMSharpTest.RDM
{
    public class RDMCurveDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMCurveDescription curveDescription = new RDMCurveDescription(1, "Pseudo Curve");
            byte[] data = curveDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.CURVE_DESCRIPTION,
                ParameterData = data,
            };

            RDMCurveDescription resultCurveDescription = RDMCurveDescription.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMCurveDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultCurveDescription, Is.EqualTo(curveDescription));

            var res = resultCurveDescription.ToString();
            var src = curveDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMCurveDescription resultCurveDescription = new RDMCurveDescription(description: "Pseudo Curve 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultCurveDescription.Description.Length, Is.EqualTo(32));
        }
    }
}