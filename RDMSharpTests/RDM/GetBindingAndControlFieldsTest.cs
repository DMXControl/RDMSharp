using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class GetBindingAndControlFieldsTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetBindingAndControlFieldsRequest getBindingAndControlFieldsRequest = new GetBindingAndControlFieldsRequest(1, new RDMUID(1233,4231414));
            byte[] data = getBindingAndControlFieldsRequest.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BINDING_CONTROL_FIELDS,
                ParameterData = data,
            };

            GetBindingAndControlFieldsRequest resultGetBindingAndControlFieldsRequest = GetBindingAndControlFieldsRequest.FromMessage(message);

            Assert.AreEqual(getBindingAndControlFieldsRequest, resultGetBindingAndControlFieldsRequest);

            GetBindingAndControlFieldsResponse getBindingAndControlFieldsResponse = new GetBindingAndControlFieldsResponse(1, new RDMUID(1213,34444),1234, new RDMUID(542, 476436));
            data = getBindingAndControlFieldsResponse.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BINDING_CONTROL_FIELDS,
                ParameterData = data,
            };

            GetBindingAndControlFieldsResponse resultGetBindingAndControlFieldsResponse = GetBindingAndControlFieldsResponse.FromMessage(message);

            Assert.AreEqual(getBindingAndControlFieldsResponse, resultGetBindingAndControlFieldsResponse);
        }
    }
}