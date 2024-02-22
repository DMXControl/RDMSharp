using System.Net;

namespace RDMSharpTest.RDM
{
    public class GetSetIPv4NameServerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIPv4NameServer getSetIPv4NameServer = new GetSetIPv4NameServer(1, IPAddress.Parse("2.0.0.1"));
            byte[] data = getSetIPv4NameServer.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DNS_IPV4_NAME_SERVER,
                ParameterData = data,
            };

            GetSetIPv4NameServer resultGetSetIPv4NameServer = GetSetIPv4NameServer.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetSetIPv4NameServer.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetIPv4NameServer, Is.EqualTo(getSetIPv4NameServer));

            var res = resultGetSetIPv4NameServer.ToString();
            var src = getSetIPv4NameServer.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}