using System.Net;

namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(Exception), () => { GetIPv4CurrentAddressResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });
            Assert.Throws(typeof(Exception), () => { new GetIPv4CurrentAddressResponse(1, IPAddress.Parse("192.168.0.1"), 33, ERDM_DHCPStatusMode.INACTIVE); });

            Assert.That(resultGetIPv4CurrentAddressResponse, Is.EqualTo(getIPv4CurrentAddressResponse));

            var res = resultGetIPv4CurrentAddressResponse.ToString();
            var src = getIPv4CurrentAddressResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}