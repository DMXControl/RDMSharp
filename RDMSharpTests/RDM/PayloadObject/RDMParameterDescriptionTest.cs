namespace RDMSharpTests.RDM.PayloadObject
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
            RDMParameterDescription parameterDescription = new RDMParameterDescription(32, 0, ERDM_DataType.SIGNED_WORD, ERDM_CommandClass.GET | ERDM_CommandClass.SET, 0, ERDM_SensorUnit.NEWTON, ERDM_UnitPrefix.KILO, 0, 5666669, 100, "Pseudo Parameter");
            byte[] data = parameterDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PARAMETER_DESCRIPTION,
                ParameterData = data,
            };

            RDMParameterDescription resultParameterDescription = RDMParameterDescription.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMParameterDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultParameterDescription, Is.EqualTo(parameterDescription));

            var res = resultParameterDescription.ToString();
            var src = parameterDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }

        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMParameterDescription parameterDescription = new RDMParameterDescription(2, description: "Pseudo Parameter 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(parameterDescription.Description.Length, Is.EqualTo(32));

            parameterDescription = new RDMParameterDescription(4, description: "");
            Assert.That(string.IsNullOrEmpty(parameterDescription.Description), Is.True);
        }
    }
}