using System.Net;

namespace RDMSharpTest.RDM
{
    public class GetSetIPv4StaticAddressTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIPv4StaticAddress getSetIPv4StaticAddress = new GetSetIPv4StaticAddress(1, IPAddress.Parse("2.0.0.1"), 8);
            byte[] data = getSetIPv4StaticAddress.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IPV4_STATIC_ADDRESS,
                ParameterData = data,
            };

            GetSetIPv4StaticAddress resultGetSetIPv4StaticAddress = GetSetIPv4StaticAddress.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetSetIPv4StaticAddress.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });
            Assert.Throws(typeof(Exception), () => { new GetSetIPv4StaticAddress(1, IPAddress.Parse("2.0.0.1"), 33); });

            Assert.That(resultGetSetIPv4StaticAddress, Is.EqualTo(getSetIPv4StaticAddress));

            var res = resultGetSetIPv4StaticAddress.ToString();
            var src = getSetIPv4StaticAddress.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}