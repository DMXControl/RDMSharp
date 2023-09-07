using RDMSharp;

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

            Assert.AreEqual(curve, resultCurve);
        }
    }
}