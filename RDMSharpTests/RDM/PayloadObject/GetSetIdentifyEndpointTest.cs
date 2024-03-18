namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetSetIdentifyEndpointTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIdentifyEndpoint getSetIPv4StaticAddress = new GetSetIdentifyEndpoint(1, true);
            byte[] data = getSetIPv4StaticAddress.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IDENTIFY_ENDPOINT,
                ParameterData = data,
            };

            GetSetIdentifyEndpoint resultGetSetIPv4StaticAddress = GetSetIdentifyEndpoint.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetIdentifyEndpoint.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetIPv4StaticAddress, Is.EqualTo(getSetIPv4StaticAddress));

            var res = resultGetSetIPv4StaticAddress.ToString();
            var src = getSetIPv4StaticAddress.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}