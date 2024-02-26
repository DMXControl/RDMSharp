namespace RDMSharpTests.RDM.PayloadObject
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
            GetBindingAndControlFieldsRequest getBindingAndControlFieldsRequest = new GetBindingAndControlFieldsRequest(1, new RDMUID(1233, 4231414));
            byte[] data = getBindingAndControlFieldsRequest.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BINDING_CONTROL_FIELDS,
                ParameterData = data,
            };

            GetBindingAndControlFieldsRequest resultGetBindingAndControlFieldsRequest = GetBindingAndControlFieldsRequest.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetBindingAndControlFieldsRequest.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetBindingAndControlFieldsRequest, Is.EqualTo(getBindingAndControlFieldsRequest));

            var res = getBindingAndControlFieldsRequest.ToString();
            var src = resultGetBindingAndControlFieldsRequest.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));

            GetBindingAndControlFieldsResponse getBindingAndControlFieldsResponse = new GetBindingAndControlFieldsResponse(1, new RDMUID(1213, 34444), 1234, new RDMUID(542, 476436));
            data = getBindingAndControlFieldsResponse.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.BINDING_CONTROL_FIELDS,
                ParameterData = data,
            };

            GetBindingAndControlFieldsResponse resultGetBindingAndControlFieldsResponse = GetBindingAndControlFieldsResponse.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetBindingAndControlFieldsResponse.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetBindingAndControlFieldsResponse, Is.EqualTo(getBindingAndControlFieldsResponse));

            res = getBindingAndControlFieldsResponse.ToString();
            src = resultGetBindingAndControlFieldsResponse.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}