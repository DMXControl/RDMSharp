namespace RDMSharpTest.RDM
{
    public class RDMSlotDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMSlotDescription slotDescription = new RDMSlotDescription(3, "Pseudo Desctiption");

            byte[] data = slotDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.SLOT_DESCRIPTION,
                ParameterData = data,
            };

            RDMSlotDescription resultSlotDescription = RDMSlotDescription.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMSlotDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultSlotDescription, Is.EqualTo(slotDescription));

            var res = resultSlotDescription.ToString();
            var src = slotDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}