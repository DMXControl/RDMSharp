namespace RDMSharpTest.RDM
{
    public class GetSetIPV4_xxx_ModeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetIPV4_xxx_Mode getSetDHCPMode = new GetSetIPV4_xxx_Mode(1, true);
            byte[] data = getSetDHCPMode.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.IPV4_DHCP_MODE,
                ParameterData = data,
            };

            GetSetIPV4_xxx_Mode resultGetSetDHCPMode = GetSetIPV4_xxx_Mode.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { GetSetIPV4_xxx_Mode.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultGetSetDHCPMode, Is.EqualTo(getSetDHCPMode));

            var res = resultGetSetDHCPMode.ToString();
            var src = getSetDHCPMode.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}