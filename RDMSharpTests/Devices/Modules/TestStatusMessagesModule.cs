using RDMSharp.Metadata;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules
{
    public class TestStatusMessagesModule
    {
        private MockGeneratedDevice1? generated;

        private static UID CONTROLLER_UID = new UID(0x1fff, 333);
        private static UID DEVCIE_UID = new UID(123, 555);
        [SetUp]
        public void Setup()
        {
            var defines = MetadataFactory.GetMetadataDefineVersions();
            generated = new MockGeneratedDevice1(DEVCIE_UID);
        }
        [TearDown]
        public void TearDown()
        {
            generated?.Dispose();
            generated = null;
        }

        [Test, Order(100)]
        public void TestGetSTATUS_MESSAGES()
        {
            #region Test Empty Status Messages
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.STATUS_MESSAGES,
                SubDevice = SubDevice.Root,
                ParameterData = new byte[] { (byte)ERDM_Status.ERROR }
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            #endregion

            #region Test Basic Status Messages
            generated.AddStatusMessage(new RDMStatusMessage(
                subDeviceId: 0,
                statusType: ERDM_Status.ERROR,
                statusMessage: ERDM_StatusMessage.OVERCURRENT,
                dataValue1: 1234,
                dataValue2: 5678));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            RDMStatusMessage[] statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages[0], Is.EqualTo(generated.StatusMessages[0]));

            generated.AddStatusMessage(new RDMStatusMessage(
                subDeviceId: 0,
                statusType: ERDM_Status.ERROR,
                statusMessage: ERDM_StatusMessage.UNDERTEMP,
                dataValue1: 33,
                dataValue2: 12));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(18));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages, Has.Length.EqualTo(2));
            Assert.That(statusMessages[0], Is.EqualTo(generated.StatusMessages[0]));
            Assert.That(statusMessages[1], Is.EqualTo(generated.StatusMessages[1]));
            #endregion

            #region Test Overflow
            for (byte i = 0; i < 30; i++)
            {
                generated.AddStatusMessage(new RDMStatusMessage(
                    subDeviceId: i,
                    statusType: ERDM_Status.ADVISORY,
                    statusMessage: ERDM_StatusMessage.WATTS,
                    dataValue1: 33,
                    dataValue2: 12));
            }

            request.ParameterData = new byte[] { (byte)ERDM_Status.ADVISORY };
            response = generated.ProcessRequestMessage_Internal(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK_OVERFLOW));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9 * 25));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages, Has.Length.EqualTo(25));
            for (int i = 0; i < 25; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i]));

            response = generated.ProcessRequestMessage_Internal(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9 * 7));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages, Has.Length.EqualTo(7));
            for (int i = 0; i < 7; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i + 25]));
            #endregion

            #region Test GET_LAST_MESSAGE
            request.ParameterData = new byte[] { (byte)ERDM_Status.GET_LAST_MESSAGE };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9 * 7));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages, Has.Length.EqualTo(7));
            for (int i = 0; i < 7; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i + 25]));
            #endregion

            #region Test filtering by status type
            request.ParameterData = new byte[] { (byte)ERDM_Status.ERROR };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(18));
            statusMessages = (RDMStatusMessage[])response.Value;
            for (int i = 0; i < 2; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i]));
            #endregion

            #region Test Cleared Status Messages
            generated.ClearStatusMessage(generated.StatusMessages[0]);
            request.ParameterData = new byte[] { (byte)ERDM_Status.ERROR };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(18));
            statusMessages = (RDMStatusMessage[])response.Value;
            Assert.That(statusMessages[0].EStatusType, Is.EqualTo(ERDM_Status.ERROR_CLEARED));
            for (int i = 0; i < 2; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i]));
            #endregion

            #region Test Remove Status Messages
            foreach (RDMStatusMessage statusMessage in generated.StatusMessages.Values.ToArray())
                generated.RemoveStatusMessage(statusMessage);

            request.ParameterData = new byte[] { (byte)ERDM_Status.ADVISORY };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            #endregion


            #region CLEAR_STATUS_ID
            generated.AddStatusMessage(new RDMStatusMessage(
                subDeviceId: 0,
                statusType: ERDM_Status.ERROR,
                statusMessage: ERDM_StatusMessage.OVERCURRENT,
                dataValue1: 1234,
                dataValue2: 5678));
            request.ParameterData = new byte[] { };
            request.Parameter = ERDM_Parameter.CLEAR_STATUS_ID;
            request.Command = ERDM_Command.SET_COMMAND;
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.CLEAR_STATUS_ID));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            #endregion
        }
    }
}