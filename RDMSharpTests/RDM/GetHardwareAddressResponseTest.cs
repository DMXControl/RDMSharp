using RDMSharp;
using NUnit.Framework;

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
            GetHardwareAddressResponse getHardwareAddressResponse = new GetHardwareAddressResponse(1,MACAddress.Parse("02:42:c0:a8:01:09"));
            byte[] data = getHardwareAddressResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.INTERFACE_HARDWARE_ADDRESS_TYPE,
                ParameterData = data,
            };

            GetHardwareAddressResponse resultGetHardwareAddressResponse = GetHardwareAddressResponse.FromMessage(message);

            Assert.AreEqual(getHardwareAddressResponse, resultGetHardwareAddressResponse);
        }
    }
}