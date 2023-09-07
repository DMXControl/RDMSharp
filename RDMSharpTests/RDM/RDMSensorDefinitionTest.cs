using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
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

            Assert.AreEqual(sensorDefinition, resultSensorDefinition);
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMSensorDefinition sensorDefinition = new RDMSensorDefinition(description: "Pseudo Sensor 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.AreEqual(32, sensorDefinition.Description.Length);
        }
    }
}