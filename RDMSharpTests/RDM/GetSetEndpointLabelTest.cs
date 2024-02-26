namespace RDMSharpTest.RDM
{
    public class GetSetEndpointLabelTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetEndpointLabel getSetEndpointLabel = new GetSetEndpointLabel(1, "Pseudo Endpoint Label");
            byte[] data = getSetEndpointLabel.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.ENDPOINT_LABEL,
                ParameterData = data,
            };

            GetSetEndpointLabel resultGetSetEndpointLabel = GetSetEndpointLabel.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetSetEndpointLabel.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultGetSetEndpointLabel, Is.EqualTo(getSetEndpointLabel));

            var res = resultGetSetEndpointLabel.ToString();
            var src = getSetEndpointLabel.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetSetEndpointLabel resultGetSetEndpointLabel = new GetSetEndpointLabel(endpointLabel: "Pseudo Endpoint Label 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultGetSetEndpointLabel.EndpointLabel.Length, Is.EqualTo(32));
            resultGetSetEndpointLabel = new GetSetEndpointLabel(endpointLabel: "");
            Assert.That(string.IsNullOrWhiteSpace(resultGetSetEndpointLabel.EndpointLabel), Is.True);
        }
    }
}