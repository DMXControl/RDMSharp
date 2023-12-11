using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetEndpointRespondersResponseTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetEndpointRespondersResponse getEndpointRespondersResponse = new GetEndpointRespondersResponse(0x12345678,
                new RDMUID(123, 34567872),
                new RDMUID(654, 26323133),
                new RDMUID(932, 14567542),
                new RDMUID(923, 79812414),
                new RDMUID(124, 29836472),
                new RDMUID(986, 79832421));

            byte[] data = getEndpointRespondersResponse.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_RESPONDERS,
                ParameterData = data,
            };

            GetEndpointRespondersResponse resultGetEndpointRespondersResponse = GetEndpointRespondersResponse.FromMessage(message);

            Assert.That(resultGetEndpointRespondersResponse, Is.EqualTo(getEndpointRespondersResponse));
        }
    }
}