namespace RDMSharpTest.RDM
{
    public class RDMDMX_xxxx_ModeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDMX_xxxx_Mode dmxFailMode = new RDMDMX_xxxx_Mode(55, 42, 11880);

            byte[] data = dmxFailMode.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DMX_FAIL_MODE,
                ParameterData = data,
            };

            RDMDMX_xxxx_Mode resultDMXFailMode = RDMDMX_xxxx_Mode.FromMessage(message);

            Assert.That(resultDMXFailMode, Is.EqualTo(dmxFailMode));
        }
    }
}