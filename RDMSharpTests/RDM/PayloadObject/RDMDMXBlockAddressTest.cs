namespace RDMSharpTests.RDM.PayloadObject
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
            RDMDMXBlockAddress dmxBlockAddress = new RDMDMXBlockAddress(3, 443);

            byte[] data = dmxBlockAddress.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DMX_BLOCK_ADDRESS,
                ParameterData = data,
            };

            RDMDMXBlockAddress resultDMXBlockAddress = RDMDMXBlockAddress.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { RDMDMXBlockAddress.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultDMXBlockAddress, Is.EqualTo(dmxBlockAddress));

            var res = resultDMXBlockAddress.ToString();
            var src = dmxBlockAddress.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(src, Is.Not.Null);
            });
            Assert.That(res, Is.EqualTo(src));
        }
    }
}