namespace RDMSharpTest.RDM
{
    public class RDMLockStateDescriptionTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            RDMLockStateDescription lockStateDescription = new RDMLockStateDescription(1, "Pseudo LockState");
            byte[] data = lockStateDescription.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.LOCK_STATE_DESCRIPTION,
                ParameterData = data,
            };

            RDMLockStateDescription resultLockStateDescription = RDMLockStateDescription.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { RDMLockStateDescription.FromPayloadData(data.ToList().Concat(new byte[30]).ToArray()); });

            Assert.That(resultLockStateDescription, Is.EqualTo(lockStateDescription));

            var res = resultLockStateDescription.ToString();
            var src = lockStateDescription.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            RDMLockStateDescription resultLockStateDescription = new RDMLockStateDescription(description: "Pseudo LockState 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0");
            Assert.That(resultLockStateDescription.Description.Length, Is.EqualTo(32));

            resultLockStateDescription = new RDMLockStateDescription(7, description: "");
            Assert.That(string.IsNullOrEmpty(resultLockStateDescription.Description), Is.True);
            Assert.That(resultLockStateDescription.MinIndex, Is.EqualTo(1));
            Assert.That(resultLockStateDescription.Index, Is.EqualTo(7));
        }
    }
}