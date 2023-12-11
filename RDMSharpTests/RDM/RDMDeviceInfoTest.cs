using RDMSharp;

namespace RDMSharpTest.RDM
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

            Assert.That(deviceInfo, Is.EqualTo(resultDeviceInfo));
        }
    }
}