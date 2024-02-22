namespace RDMSharpTest.RDM
{
    public class GetHardwareAddressResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetHardwareAddressResponse getHardwareAddressResponse = new GetHardwareAddressResponse(1, new MACAddress("02:42:c0:a8:01:09"));
            byte[] data = getHardwareAddressResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
                ParameterData = data,
            };

            GetHardwareAddressResponse resultGetHardwareAddressResponse = GetHardwareAddressResponse.FromMessage(message);

            Assert.That(resultGetHardwareAddressResponse, Is.EqualTo(getHardwareAddressResponse));

            var res = resultGetHardwareAddressResponse.ToString();
            var src = getHardwareAddressResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}