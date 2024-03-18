using System.Net;

namespace RDMSharpTests.RDM.PayloadObject
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
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetComponentScope.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSensorValue, Is.EqualTo(sensorValue));
            sensorValue = new GetSetComponentScope(0, "Pseudo Scope String", IPAddress.Parse("2001:db8:0:0:0:0:1428:57ab"), 2347);

            data = sensorValue.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMPONENT_SCOPE,
                ParameterData = data,
            };

            resultSensorValue = GetSetComponentScope.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetComponentScope.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSensorValue, Is.EqualTo(sensorValue));
            sensorValue = new GetSetComponentScope(0, string.Empty, null, 2347);

            data = sensorValue.ToPayloadData();

            message = new RDMMessage()
            {
                PortID_or_Responsetype = (byte)ERDM_ResponseType.ACK,
                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.COMPONENT_SCOPE,
                ParameterData = data,
            };

            resultSensorValue = GetSetComponentScope.FromMessage(message);
            Assert.Throws(typeof(RDMMessageInvalidPDLException), () => { GetSetComponentScope.FromPayloadData(data.ToList().Concat(new byte[1]).ToArray()); });

            Assert.That(resultSensorValue, Is.EqualTo(sensorValue));

            var res = resultSensorValue.ToString();
            var src = sensorValue.ToString();
            Assert.That(res, Is.Not.Null);
            Assert.That(src, Is.Not.Null);
            Assert.That(res, Is.EqualTo(src));
        }
        [Test]
        public void DescriptionCharLimitTest()
        {
            GetSetComponentScope resultGetSetComponentScope = new GetSetComponentScope(0, "Pseudo Scope String 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0", IPAddress.Parse("192.168.2.1"), 2347);
            Assert.That(resultGetSetComponentScope.ScopeString, Has.Length.EqualTo(62));

            resultGetSetComponentScope = new GetSetComponentScope(0, "Pseudo Scope String", null, 2347);
            Assert.That(resultGetSetComponentScope.StaticConfigType, Is.EqualTo(ERDM_StaticConfig.NO));

            Assert.Throws(typeof(ArgumentException), () => { new GetSetComponentScope(0, "Pseudo Scope String", ERDM_StaticConfig.IPv4, staticBrokerIPv4: IPAddress.Parse("2001:db8:0:0:0:0:1428:57ab")); });
            Assert.Throws(typeof(ArgumentException), () => { new GetSetComponentScope(0, "Pseudo Scope String", ERDM_StaticConfig.IPv6, staticBrokerIPv6: IPAddress.Parse("192.168.2.1")); });
        }
    }
}