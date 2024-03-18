namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMDeviceInfoTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDeviceInfo deviceInfo = new RDMDeviceInfo(1, 12, 333, ERDM_ProductCategoryCoarse.FIXTURE, ERDM_ProductCategoryFine.FIXTURE_MOVING_YOKE, 2344777890, 30, 8, 27, 6, 99, 42);
            byte[] data = deviceInfo.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DEVICE_INFO,
                ParameterData = data,
            };

            RDMDeviceInfo resultDeviceInfo = RDMDeviceInfo.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMDeviceInfo.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultDeviceInfo, Is.EqualTo(deviceInfo));

            var res = resultDeviceInfo.ToString();
            var src = deviceInfo.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}