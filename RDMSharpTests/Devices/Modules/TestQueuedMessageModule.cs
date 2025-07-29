using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules
{
    public class TestQueuedMessageModule
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

        [Test]
        public void TestGetQUEUED_MESSAGE()
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

            #region Test Get last Message
            request.ParameterData = new byte[] { (byte)ERDM_Status.GET_LAST_MESSAGE };
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

            request.ParameterData = new byte[] { (byte)ERDM_Status.ERROR };
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
            var deviceLabelModule = generated.Modules.OfType<DeviceLabelModule>().FirstOrDefault();
            Assert.That(deviceLabelModule, Is.Not.Null);
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("Dummy Device 1"));
            deviceLabelModule.DeviceLabel = "Test Device Queued Message 1";
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("Test Device Queued Message 1"));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(deviceLabelModule.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(deviceLabelModule.DeviceLabel));
            #endregion

            #region Test set Multiple Parameter at once
            deviceLabelModule.DeviceLabel = "GG";
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("GG"));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(deviceLabelModule.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(deviceLabelModule.DeviceLabel));

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
            Assert.That(response.Value, Is.EqualTo(generated.Personalities.ElementAt(1).Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));

            for (byte b = 0; b < generated.Personalities.ElementAt(1).SlotCount; b++)
            {
                var slot = generated.Personalities.ElementAt(1).Slots[b];
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.MessageCounter, Is.EqualTo(9 - b));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(generated.Personalities.ElementAt(1).Slots[b].Description.Length + 2));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Personalities.ElementAt(1).Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Personalities.ElementAt(1).Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));

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

            request.ParameterData = new byte[] { (byte)ERDM_Status.ADVISORY };
            var sm = new RDMStatusMessage(0, ERDM_Status.ADVISORY, ERDM_StatusMessage.AMPS, 222);
            generated.AddStatusMessage(sm);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.STATUS_MESSAGES));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(9));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMStatusMessage[])));
            RDMStatusMessage[] messages = (RDMStatusMessage[])response.Value;
            Assert.That(messages[0], Is.EqualTo(sm));

            #region
            request.ParameterData = new byte[] { (byte)ERDM_Status.NONE };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.QUEUED_MESSAGE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.MessageCounter, Is.EqualTo(0));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.FORMAT_ERROR }));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            #endregion

            #region CLEAR_STATUS_ID
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