namespace RDMSharpTest.RDM
{
    public class SetLockPinRequestTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            SetLockPinRequest setLockPinRequest = new SetLockPinRequest(11880, 0815);

            byte[] data = setLockPinRequest.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.SET_COMMAND,
                Parameter = ERDM_Parameter.LOCK_PIN,
                ParameterData = data,
            };

            SetLockPinRequest resultSetLockPinRequest = SetLockPinRequest.FromMessage(message);

            Assert.That(resultSetLockPinRequest, Is.EqualTo(setLockPinRequest));

            var res = resultSetLockPinRequest.ToString();
            var src = setLockPinRequest.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
    }
}