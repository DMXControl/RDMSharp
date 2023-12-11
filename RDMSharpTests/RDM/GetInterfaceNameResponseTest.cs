using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetInterfaceNameResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetInterfaceNameResponse getInterfaceNameResponse = new GetInterfaceNameResponse(1, "Pseude Interface");
            byte[] data = getInterfaceNameResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.INTERFACE_LABEL,
                ParameterData = data,
            };

            GetInterfaceNameResponse resultGetInterfaceNameResponse = GetInterfaceNameResponse.FromMessage(message);

            Assert.That(resultGetInterfaceNameResponse, Is.EqualTo(getInterfaceNameResponse));
        }
    }
}