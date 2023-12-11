using RDMSharp;
using NUnit.Framework;

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

            Assert.That(resultOutputResponseTime, Is.EqualTo(outputResponseTime));
        }
    }
}