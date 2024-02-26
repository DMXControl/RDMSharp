namespace RDMSharpTests.RDM.PayloadObject
{
    public class SetLockStateRequestTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            SetLockStateRequest setLockStateRequest = new SetLockStateRequest(11880, 1);

            byte[] data = setLockStateRequest.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.SET_COMMAND,
                Parameter = ERDM_Parameter.LOCK_STATE,
                ParameterData = data,
            };

            SetLockStateRequest resultSetLockStateRequest = SetLockStateRequest.FromMessage(message);
            Assert.Throws(typeof(Exception), () => { SetLockStateRequest.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSetLockStateRequest, Is.EqualTo(setLockStateRequest));

            var res = resultSetLockStateRequest.ToString();
            var src = setLockStateRequest.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}