namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMRealTimeClockTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMRealTimeClock realTimeClock = new RDMRealTimeClock(DateTime.Now);

            byte[] data = realTimeClock.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.REAL_TIME_CLOCK,
                ParameterData = data,
            };

            RDMRealTimeClock resultRealTimeClock = RDMRealTimeClock.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMRealTimeClock.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultRealTimeClock, Is.EqualTo(realTimeClock));

            var res = resultRealTimeClock.ToString();
            var src = realTimeClock.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(year: 2000); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(month: 13); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(day: 32); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(hour: 25); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(minute: 60); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(second: 60); });
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new RDMRealTimeClock(year: 2000, month: 2, day: 30); });
        }
    }
}