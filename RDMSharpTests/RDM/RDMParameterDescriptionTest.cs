using RDMSharp;

namespace RDMSharpTest.RDM
{
    public class RDMParameterDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMParameterDescription parameterDescription = new RDMParameterDescription(32, 0, ERDM_DataType.SIGNED_WORD, ERDM_CommandClass.GET| ERDM_CommandClass.SET, 0, ERDM_SensorUnit.NEWTON, ERDM_UnitPrefix.KILO, 0, 5666669, 100, "Pseudo Parameter");
            byte[] data = parameterDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PARAMETER_DESCRIPTION,
                ParameterData = data,
            };

            RDMParameterDescription resultParameterDescription = RDMParameterDescription.FromMessage(message);

            Assert.AreEqual(parameterDescription, resultParameterDescription);
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMParameterDescription parameterDescription = new RDMParameterDescription(description: "Pseudo Parameter 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.AreEqual(32, parameterDescription.Description.Length);
        }
    }
}