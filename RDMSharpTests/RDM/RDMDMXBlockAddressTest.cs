using RDMSharp;
using NUnit.Framework;

namespace RDMSharpTest.RDM
{
    public class RDMDMXBlockAddressTEst
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDMXBlockAddress dmxBlockAddress = new RDMDMXBlockAddress(3,443);

            byte[] data = dmxBlockAddress.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DMX_BLOCK_ADDRESS,
                ParameterData = data,
            };

            RDMDMXBlockAddress resultDMXBlockAddress = RDMDMXBlockAddress.FromMessage(message);

            Assert.That(resultDMXBlockAddress, Is.EqualTo(dmxBlockAddress));
        }
    }
}