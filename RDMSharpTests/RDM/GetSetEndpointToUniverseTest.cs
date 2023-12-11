using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetSetEndpointToUniverseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointToUniverse getSetEndpointToUniverse = new GetSetEndpointToUniverse(1, 1234);
            byte[] data = getSetEndpointToUniverse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_TO_UNIVERSE,
                ParameterData = data,
            };

            GetSetEndpointToUniverse resultGetSetEndpointToUniverse = GetSetEndpointToUniverse.FromMessage(message);

            Assert.That(resultGetSetEndpointToUniverse, Is.EqualTo(getSetEndpointToUniverse));
        }
    }
}