namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMProxiedDevicesTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMProxiedDevices proxiedDevices = new RDMProxiedDevices(
                new RDMUID(214567834),
                new RDMUID(2723737),
                new RDMUID(5959076060),
                new RDMUID(067060),
                new RDMUID(32490538486848484));

            byte[] data = proxiedDevices.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PROXIED_DEVICES,
                ParameterData = data,
            };

            RDMProxiedDevices resultproxiedDevices = RDMProxiedDevices.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMProxiedDevices.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMProxiedDevices.FromPayloadData(data.ToList().Concat(new byte[220]).ToArray()); });

            Assert.That(resultproxiedDevices, Is.EqualTo(proxiedDevices));

            var res = resultproxiedDevices.ToString();
            var src = proxiedDevices.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}