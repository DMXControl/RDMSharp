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
            RDMSensorValue sensorValue = new RDMSensorValue(5, 12359, 12347, 10101, 11880);

            byte[] data = sensorValue.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SENSOR_VALUE,
                ParameterData = data,
            };

            RDMSensorValue resultSensorValue = RDMSensorValue.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMSensorValue.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSensorValue, Is.EqualTo(sensorValue));
            Assert.That(resultSensorValue.MinIndex, Is.EqualTo(0));
            Assert.That(resultSensorValue.Index, Is.EqualTo(5));

            var res = resultSensorValue.ToString();
            var src = sensorValue.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}