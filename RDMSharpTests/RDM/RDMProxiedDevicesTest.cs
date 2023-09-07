using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
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
                RDMUID.FromULong(214567834),
                RDMUID.FromULong(2723737),
                RDMUID.FromULong(5959076060),
                RDMUID.FromULong(067060),
                RDMUID.FromULong(32490538486848484));

            byte[] data = proxiedDevices.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.PROXIED_DEVICES,
                ParameterData = data,
            };

            RDMProxiedDevices resultproxiedDevices = RDMProxiedDevices.FromMessage(message);

            Assert.AreEqual(proxiedDevices, resultproxiedDevices);
        }
    }
}