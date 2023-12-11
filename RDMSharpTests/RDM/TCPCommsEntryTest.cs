using System.Net;

namespace RDMSharpTest.RDM
{
    public class TCPCommsEntryTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            TCPCommsEntry tcpCommsEntryTest = new TCPCommsEntry("Pseudo TCPCommsEntryTest", IPAddress.Parse("192.168.2.1"), 2347, 77);

            byte[] data = tcpCommsEntryTest.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.TCP_COMMS_STATUS,
                ParameterData = data,
            };

            TCPCommsEntry resultTCPCommsEntryTest = TCPCommsEntry.FromMessage(message);

            Assert.That(resultTCPCommsEntryTest, Is.EqualTo(tcpCommsEntryTest)); tcpCommsEntryTest = new TCPCommsEntry("Pseudo TCPCommsEntryTest", IPAddress.Parse("2001:db8:0:0:0:0:1428:57ab"), 2347, 99);

            data = tcpCommsEntryTest.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.TCP_COMMS_STATUS,
                ParameterData = data,
            };

            resultTCPCommsEntryTest = TCPCommsEntry.FromMessage(message);

            Assert.That(resultTCPCommsEntryTest, Is.EqualTo(tcpCommsEntryTest));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            TCPCommsEntry resultGetSetComponentScope = new TCPCommsEntry("Pseudo TCPCommsEntryTest 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0", IPAddress.Parse("192.168.2.1"), 2347);
            Assert.That(resultGetSetComponentScope.ScopeString.Length, Is.EqualTo(62));
        }
    }
}