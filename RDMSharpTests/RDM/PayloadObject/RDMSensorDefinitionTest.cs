namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMSensorDefinitionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMSensorDefinition sensorDefinition = new RDMSensorDefinition(0, ERDM_SensorType.ACCELERATION, ERDM_SensorUnit.DEGREE, ERDM_UnitPrefix.ATTO, short.MinValue, short.MaxValue, short.MinValue, short.MaxValue, true, false, "Pseudo Sensor");

            byte[] data = sensorDefinition.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SENSOR_DEFINITION,
                ParameterData = data,
            };

            RDMSensorDefinition resultSensorDefinition = RDMSensorDefinition.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMSensorDefinition.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultSensorDefinition, Is.EqualTo(sensorDefinition));

            var res = resultSensorDefinition.ToString();
            var src = sensorDefinition.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMSensorDefinition sensorDefinition = new RDMSensorDefinition(description: "Pseudo Sensor 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(sensorDefinition.Description, Has.Length.EqualTo(32));

            sensorDefinition = new RDMSensorDefinition(3, description: "");
            Assert.Multiple(() =>
            {
                Assert.That(string.IsNullOrEmpty(sensorDefinition.Description), Is.True);
                Assert.That(sensorDefinition.MinIndex, Is.EqualTo(0));
                Assert.That(sensorDefinition.Index, Is.EqualTo(3));
            });
        }
    }
}