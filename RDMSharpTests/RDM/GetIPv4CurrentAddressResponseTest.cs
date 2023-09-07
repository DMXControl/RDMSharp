using RDMSharp;
using NUnit.Framework;
using System.Net;

namespace RDMSharpTest.RDM
{
    public class GetIPv4CurrentAddressResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetIPv4CurrentAddressResponse getIPv4CurrentAddressResponse = new GetIPv4CurrentAddressResponse(1, IPAddress.Parse("192.168.0.1"), 24, ERDM_DHCPStatusMode.INACTIVE);
            byte[] data = getIPv4CurrentAddressResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IPV4_CURRENT_ADDRESS,
                ParameterData = data,
            };

            GetIPv4CurrentAddressResponse resultGetIPv4CurrentAddressResponse = GetIPv4CurrentAddressResponse.FromMessage(message);

            Assert.AreEqual(getIPv4CurrentAddressResponse, resultGetIPv4CurrentAddressResponse);
        }
    }
}