using System.Net;

namespace RDMSharpTests.RDM.PayloadObject
{
    public class GetSetIPv4DefaultRouteTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIPv4DefaultRoute getSetIPv4DefaultRoute = new GetSetIPv4DefaultRoute(1, IPAddress.Parse("2.0.0.1"));
            byte[] data = getSetIPv4DefaultRoute.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IPV4_DEFAULT_ROUTE,
                ParameterData = data,
            };

            GetSetIPv4DefaultRoute resultGetSetIPv4DefaultRoute = GetSetIPv4DefaultRoute.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetIPv4DefaultRoute.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetIPv4DefaultRoute, Is.EqualTo(getSetIPv4DefaultRoute));

            var res = resultGetSetIPv4DefaultRoute.ToString();
            var src = getSetIPv4DefaultRoute.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}