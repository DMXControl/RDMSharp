using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetEndpointListResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointListResponse getEndpointListResponse = new GetEndpointListResponse(0x12345678,
                new EndpointDescriptor(),
                new EndpointDescriptor(1, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(1),
                new EndpointDescriptor(1, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(2),
                new EndpointDescriptor(2, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(2),
                new EndpointDescriptor(3, ERDM_EndpointType.PHYSICAL),
                new EndpointDescriptor(3),
                new EndpointDescriptor(3, ERDM_EndpointType.PHYSICAL));

            byte[] data = getEndpointListResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_LIST,
                ParameterData = data,
            };

            GetEndpointListResponse resultGetEndpointListResponse = GetEndpointListResponse.FromMessage(message);

            Assert.AreEqual(getEndpointListResponse, resultGetEndpointListResponse);
        }
    }
}