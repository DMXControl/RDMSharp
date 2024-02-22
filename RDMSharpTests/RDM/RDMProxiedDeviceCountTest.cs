namespace RDMSharpTest.RDM
{
    public class RDMProxiedDeviceCountTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMProxiedDeviceCount proxiedDeviceCount = new RDMProxiedDeviceCount(10, true);

            byte[] data = proxiedDeviceCount.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PROXIED_DEVICES_COUNT,
                ParameterData = data,
            };

            RDMProxiedDeviceCount resultProxiedDeviceCount = RDMProxiedDeviceCount.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMProxiedDeviceCount.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultProxiedDeviceCount, Is.EqualTo(proxiedDeviceCount));

            var res = resultProxiedDeviceCount.ToString();
            var src = proxiedDeviceCount.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}