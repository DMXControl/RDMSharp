namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetSetEndpointToUniverseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointToUniverse getSetEndpointToUniverse = new GetSetEndpointToUniverse(1, 1234);
            byte[] data = getSetEndpointToUniverse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
                ParameterData = data,
            };

            GetSetEndpointToUniverse resultGetSetEndpointToUniverse = GetSetEndpointToUniverse.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetEndpointToUniverse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetEndpointToUniverse, Is.EqualTo(getSetEndpointToUniverse));

            var res = resultGetSetEndpointToUniverse.ToString();
            var src = getSetEndpointToUniverse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}