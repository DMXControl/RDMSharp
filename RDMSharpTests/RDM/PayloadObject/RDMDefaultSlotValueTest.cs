namespace RDMSharpTests.RDM.PayloadObject
{
    public class RDMDefaultSlotValueTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMDefaultSlotValue defaultSlotValue = new RDMDefaultSlotValue(3, 250);

            byte[] data = defaultSlotValue.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DEFAULT_SLOT_VALUE,
                ParameterData = data,
            };

            RDMDefaultSlotValue resultDefaultSlotValue = RDMDefaultSlotValue.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMDefaultSlotValue.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultDefaultSlotValue, Is.EqualTo(defaultSlotValue));

            var res = resultDefaultSlotValue.ToString();
            var src = defaultSlotValue.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}