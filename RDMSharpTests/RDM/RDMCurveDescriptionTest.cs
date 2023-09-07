using RDMSharp;

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
            RDMCurveDescription curveDescription = new RDMCurveDescription(1,"Pseudo Curve");
            byte[] data = curveDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.CURVE_DESCRIPTION,
                ParameterData = data,
            };

            RDMCurveDescription resultCurveDescription = RDMCurveDescription.FromMessage(message);

            Assert.AreEqual(curveDescription, resultCurveDescription);
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMCurveDescription resultCurveDescription = new RDMCurveDescription(description: "Pseudo Curve 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.AreEqual(32, resultCurveDescription.Description.Length);
        }
    }
}