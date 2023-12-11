namespace RDMSharpTest.RDM
{
    public class RDMOutputResponseTimeDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMOutputResponseTimeDescription outputResponseTimeDescription = new RDMOutputResponseTimeDescription(255, "Pseudo OutputResponseTime");
            byte[] data = outputResponseTimeDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.OUTPUT_RESPONSE_TIME_DESCRIPTION,
                ParameterData = data,
            };

            RDMOutputResponseTimeDescription resultOutputResponseTimeDescription = RDMOutputResponseTimeDescription.FromMessage(message);

            Assert.That(outputResponseTimeDescription, Is.EqualTo(resultOutputResponseTimeDescription));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMCurveDescription resultOutputResponseTimeDescription = new RDMCurveDescription(description: "Pseudo OutputResponseTime 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultOutputResponseTimeDescription.Description.Length, Is.EqualTo(32));
        }
    }
}