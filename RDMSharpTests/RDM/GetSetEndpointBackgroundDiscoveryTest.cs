
using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetSetEndpointBackgroundDiscoveryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointBackgroundDiscovery getSetEndpointBackgroundDiscovery = new GetSetEndpointBackgroundDiscovery(1, true);
            byte[] data = getSetEndpointBackgroundDiscovery.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BACKGROUND_DISCOVERY,
                ParameterData = data,
            };

            GetSetEndpointBackgroundDiscovery resultGetSetEndpointBackgroundDiscovery = GetSetEndpointBackgroundDiscovery.FromMessage(message);

            Assert.AreEqual(getSetEndpointBackgroundDiscovery, resultGetSetEndpointBackgroundDiscovery);
        }
    }
}