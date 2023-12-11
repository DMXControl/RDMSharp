using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class RDMSensorValueTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMSensorValue sensorValue = new RDMSensorValue(0, 12359, 12347, 10101, 11880);

            byte[] data = sensorValue.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SENSOR_VALUE,
                ParameterData = data,
            };

            RDMSensorValue resultSensorValue = RDMSensorValue.FromMessage(message);

            Assert.That(resultSensorValue, Is.EqualTo(sensorValue));
        }
    }
}