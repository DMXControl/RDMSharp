using RDMSharp;
using NUnit.Framework;
using System.Net;

namespace RDMSharpTest.RDM
{
    public class GetSetComponentScopeTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ToPayloadAndFromMessageTest()
        {
            GetSetComponentScope sensorValue = new GetSetComponentScope(0, "Pseudo Scope String", IPAddress.Parse("192.168.2.1"), 2347);

            byte[] data = sensorValue.ToPayloadData();

            RDMMessage message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMPONENT_SCOPE,
                ParameterData = data,
            };

            GetSetComponentScope resultSensorValue = GetSetComponentScope.FromMessage(message);

            Assert.AreEqual(sensorValue, resultSensorValue); sensorValue = new GetSetComponentScope(0, "Pseudo Scope String", IPAddress.Parse("2001:db8:0:0:0:0:1428:57ab"), 2347);

            data = sensorValue.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMPONENT_SCOPE,
                ParameterData = data,
            };

            resultSensorValue = GetSetComponentScope.FromMessage(message);

            Assert.AreEqual(sensorValue, resultSensorValue);
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetSetComponentScope resultGetSetComponentScope = new GetSetComponentScope(0, "Pseudo Scope String 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0", IPAddress.Parse("192.168.2.1"), 2347);
            Assert.AreEqual(62, resultGetSetComponentScope.ScopeString.Length);
        }
    }
}