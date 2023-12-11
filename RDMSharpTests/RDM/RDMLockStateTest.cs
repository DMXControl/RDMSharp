using RDMSharp;

namespace RDMSharpTest.RDM
{
    public class RDMLockStateTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetLockStateResponse lockState = new GetLockStateResponse(1, 5);
            byte[] data = lockState.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.LOCK_STATE,
                ParameterData = data,
            };

            GetLockStateResponse resultLockState = GetLockStateResponse.FromMessage(message);

            Assert.That(resultLockState, Is.EqualTo(lockState));
        }
    }
}