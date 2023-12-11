using RDMSharp;
using NUnit.Framework;
using System;

namespace RDMSharpTest.RDM
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

            Assert.That(resultRealTimeClock, Is.EqualTo(realTimeClock));
        }
    }
}