namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetSetEndpointModeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointMode getSetEndpointMode = new GetSetEndpointMode(1, ERDM_EndpointMode.OUTPUT);
            byte[] data = getSetEndpointMode.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_MODE,
                ParameterData = data,
            };

            GetSetEndpointMode resultGetSetEndpointMode = GetSetEndpointMode.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetEndpointMode.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetEndpointMode, Is.EqualTo(getSetEndpointMode));

            var res = resultGetSetEndpointMode.ToString();
            var src = getSetEndpointMode.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));

            getSetEndpointMode = new GetSetEndpointMode((ushort)1, (byte)ERDM_EndpointMode.INPUT);
            data = getSetEndpointMode.ToPayloadData();
            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_MODE,
                ParameterData = data,
            };
            
            resultGetSetEndpointMode = GetSetEndpointMode.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetEndpointMode.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            res = resultGetSetEndpointMode.ToString();
            src = getSetEndpointMode.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}