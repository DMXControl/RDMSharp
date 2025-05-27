using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceiveGeneratedOnly
    {
        private MockGeneratedDevice1? generated;
        private Random random = new Random();
        private static UID CONTROLLER_UID = new UID(0x1fff, 333);
        private static UID DEVCIE_UID = new UID(123, 555);
        [SetUp]
        public void Setup()
        {
            generated = new MockGeneratedDevice1(DEVCIE_UID);
        }
        [TearDown]
        public void TearDown()
        {
            generated?.Dispose();
            generated = null;
        }

        [Test, Order(1)]
        public void TestGetStatusMessages()
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
            Assert.That(statusMessages[0].StatusType, Is.EqualTo(ERDM_Status.ERROR_CLEARED));
            for (int i = 0; i < 2; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i]));
            #endregion
        }
        [Test, Order(1000)]
        public void TestGetQueuedMessages()
        {
            #region Test Empty queue
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.QUEUED_MESSAGE,
                SubDevice = SubDevice.Root,
                ParameterData = new byte[] { (byte)ERDM_Status.ADVISORY }
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

            #region Test set DMX-Address (single value changed)
            Assert.That(generated.DMXAddress, Is.EqualTo(1));
            generated.DMXAddress = 42;
            Assert.That(generated.DMXAddress, Is.EqualTo(42));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_START_ADDRESS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(1));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.EqualTo(generated.DMXAddress));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(19));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceInfo));
            #endregion

            #region Test set DMX-Address (multiple value changed)
            Assert.That(generated.DMXAddress, Is.EqualTo(42));
            generated.DMXAddress = 50;
            Assert.That(generated.DMXAddress, Is.EqualTo(50));
            generated.DMXAddress = 60;
            Assert.That(generated.DMXAddress, Is.EqualTo(60));
            generated.DMXAddress = 70;
            Assert.That(generated.DMXAddress, Is.EqualTo(70));
            generated.DMXAddress = 80;
            Assert.That(generated.DMXAddress, Is.EqualTo(80));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_START_ADDRESS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(1));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.EqualTo(generated.DMXAddress));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(19));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceInfo));
            #endregion

            #region Test set DeviceLabel (single value changed)
            Assert.That(generated.DeviceLabel, Is.EqualTo("Dummy Device 1"));
            generated.DeviceLabel= "Test Device Queued Message 1";
            Assert.That(generated.DeviceLabel, Is.EqualTo("Test Device Queued Message 1"));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceLabel));
            #endregion

            #region Test set Multiple Parameter at once
            generated.DeviceLabel = "GG";
            Assert.That(generated.DeviceLabel, Is.EqualTo("GG"));
            generated.SetParameter(ERDM_Parameter.IDENTIFY_DEVICE, true);
            generated.CurrentPersonality = 2;

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(13));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceLabel));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(12));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(true));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(11));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.EqualTo(new RDMDMXPersonality(2, 3)));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(10));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(40));
            Assert.That(response.Value, Is.EqualTo(generated.Personalities[1].Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));

            for (byte b = 0; b < generated.Personalities[1].SlotCount; b++)
            {
                var slot = generated.Personalities[1].Slots[b];
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.MessageCounter, Is.EqualTo(9-b));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(generated.Personalities[1].Slots[b].Description.Length + 2));
                Assert.That(response.Value, Is.EqualTo(new RDMSlotDescription(slot.SlotId, slot.Description)));
            }

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(1));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Personalities[1].Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Personalities[1].Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(19));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceInfo));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            #endregion
        }
    }
}