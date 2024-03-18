namespace RDMSharpTests.RDM.PayloadObject
{
    public class AcknowledgeTimerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            var time = TimeSpan.FromSeconds(360000);
            AcknowledgeTimer acknowledgeTimer = new AcknowledgeTimer(time);
            byte[] data = acknowledgeTimer.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK_TIMER,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.LAMP_STRIKES,
                ParameterData = data,
            };

            AcknowledgeTimer resultAcknowledgeTimer = AcknowledgeTimer.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { AcknowledgeTimer.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultAcknowledgeTimer, Is.EqualTo(acknowledgeTimer));
            Assert.That(resultAcknowledgeTimer.EstimidatedResponseTime, Is.EqualTo(time));

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new AcknowledgeTimer(TimeSpan.FromSeconds(3600000)); });

            var res = resultAcknowledgeTimer.ToString();
            var src = acknowledgeTimer.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}