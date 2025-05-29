using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices
{
    public class TestRDMSendReceiveGeneratedOnly
    {
        private MockGeneratedDevice1? generated;

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
        public void TestGetSUPPORTED_PARAMETERS()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SUPPORTED_PARAMETERS,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SUPPORTED_PARAMETERS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.Parameters.Length * 2));
            Assert.That(response.Value, Is.TypeOf(typeof(ERDM_Parameter[])));
            var parametersRemote = ((ERDM_Parameter[])response.Value).OrderBy(p => p).ToArray();
            var parametersGenerated = generated.Parameters.OrderBy(p => p).ToArray();
            Assert.That(parametersRemote, Is.EquivalentTo(parametersGenerated));
            #endregion
        }
        [Test, Order(5)]
        public void TestGetDEVICE_INFO()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DEVICE_INFO,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(19));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceInfo));
            #endregion
        }
        [Test, Order(6)]
        public void TestGetIDENTIFY_DEVICE()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Identify, Is.False);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.IDENTIFY_DEVICE,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(generated.Identify));
            #endregion

            #region Test Label changed
            Assert.That(generated.Identify, Is.False);
            generated.Identify = true;
            Assert.That(generated.Identify, Is.True);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(1));
            Assert.That(response.Value, Is.EqualTo(generated.Identify));
            #endregion
        }
        [Test, Order(7)]
        public void TestGetDMX_START_ADDRESS()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DMX_START_ADDRESS,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_START_ADDRESS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.EqualTo(generated.DMXAddress));
            #endregion


            #region Test Address changed
            generated.DMXAddress = 40;
            Assert.That(generated.DMXAddress, Is.EqualTo(40));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_START_ADDRESS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.EqualTo(generated.DMXAddress));
            #endregion
        }
        [Test, Order(9)]
        public void TestGetDEVICE_LABEL()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.DeviceLabel, Is.EqualTo("Dummy Device 1"));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DEVICE_LABEL,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceLabel));
            #endregion

            #region Test Label changed
            Assert.That(generated.DeviceLabel, Is.EqualTo("Dummy Device 1"));
            generated.DeviceLabel = "Rem x Ram";
            Assert.That(generated.DeviceLabel, Is.EqualTo("Rem x Ram"));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceLabel));
            #endregion
        }
        [Test, Order(10)]
        public void TestGetDMX_PERSONALITY_DESCRIPTION()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION,
                SubDevice = SubDevice.Root,
                ParameterData = new byte[] { 0x00 } // Requesting invalid 0 
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.ACTION_NOT_SUPPORTED }));

            for (byte b = 0; b < generated.Personalities.Length; b++)
            {
                request.ParameterData = new byte[] { (byte)(b + 1) };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY_DESCRIPTION));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                var pers = generated.Personalities[b];
                var expected = new RDMDMXPersonalityDescription(pers.ID, pers.SlotCount, pers.Description);
                Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
                Assert.That(response.Value, Is.EqualTo(expected));
            }
            #endregion
        }
        [Test, Order(11)]
        public void TestGetDMX_PERSONALITY()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.CurrentPersonality, Is.EqualTo(1));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DMX_PERSONALITY,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            var pers = generated.Personalities[0];
            var expected = new RDMDMXPersonality(pers.ID, (byte)generated.Personalities.Count());
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            #endregion

            #region Test Label changed
            Assert.That(generated.CurrentPersonality, Is.EqualTo(1));
            generated.CurrentPersonality = 2;
            Assert.That(generated.CurrentPersonality, Is.EqualTo(2));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DMX_PERSONALITY));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            pers = generated.Personalities[1];
            expected = new RDMDMXPersonality(pers.ID, (byte)generated.Personalities.Count());
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            #endregion
        }
        [Test, Order(15)]
        public void TestGetMANUFACTURER_LABEL()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.ManufacturerLabel, Is.EqualTo("Dummy Manufacturer 9FFF"));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.MANUFACTURER_LABEL,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.MANUFACTURER_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.ManufacturerLabel.Length));
            Assert.That(response.Value, Is.EqualTo(generated.ManufacturerLabel));
            #endregion
        }
        [Test, Order(16)]
        public void TestGetDEVICE_MODEL_DESCRIPTION()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.DeviceModelDescription, Is.EqualTo("Test Model Description"));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DEVICE_MODEL_DESCRIPTION,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_MODEL_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.DeviceModelDescription.Length));
            Assert.That(response.Value, Is.EqualTo(generated.DeviceModelDescription));
            #endregion
        }
        [Test, Order(30)]
        public void TestGetBOOT_SOFTWARE_VERSION_ID()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.ParameterValues[ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID], Is.EqualTo(4660));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(generated.ParameterValues[ERDM_Parameter.BOOT_SOFTWARE_VERSION_ID]));
            #endregion
        }
        [Test, Order(30)]
        public void TestGetBOOT_SOFTWARE_VERSION_LABEL()
        {
            const string SOFTWARE_VERSION_LABEL = "Dummy Software";
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.ParameterValues[ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL], Is.EqualTo(SOFTWARE_VERSION_LABEL));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(SOFTWARE_VERSION_LABEL.Length));
            Assert.That(response.Value, Is.EqualTo(SOFTWARE_VERSION_LABEL));
            #endregion
        }

        [Test, Order(40)]
        public void TestGetSLOT_INFO()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Slots, Has.Count.EqualTo(5));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SLOT_INFO,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
            #endregion

            #region Test Change Personality
            Assert.That(generated, Is.Not.Null);
            generated.CurrentPersonality = 2; // Change to personality 2
            Assert.That(generated.Slots, Has.Count.EqualTo(8));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
            #endregion

            #region Test Change Personality
            Assert.That(generated, Is.Not.Null);
            generated.CurrentPersonality = 3; // Change to personality 3
            Assert.That(generated.Slots, Has.Count.EqualTo(9));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_INFO));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(5 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMSlotInfo(s.Value.SlotId, s.Value.Type, s.Value.Category))));
            #endregion

            #region Test Invalid Calls
            Assert.Throws(typeof(NullReferenceException), () => generated.CurrentPersonality = null); // Change to personality null
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 0); // Change to personality 0
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 4); // Change to personality 4
            Assert.DoesNotThrow(() => generated.CurrentPersonality = 3); // Change to personality 3
            #endregion
        }

        [Test, Order(41)]
        public void TestGetDEFAULT_SLOT_VALUE()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Slots, Has.Count.EqualTo(5));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.DEFAULT_SLOT_VALUE,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
            #endregion

            #region Test Change Personality
            Assert.That(generated, Is.Not.Null);
            generated.CurrentPersonality = 2; // Change to personality 2
            Assert.That(generated.Slots, Has.Count.EqualTo(8));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
            #endregion

            #region Test Change Personality
            Assert.That(generated, Is.Not.Null);
            generated.CurrentPersonality = 3; // Change to personality 3
            Assert.That(generated.Slots, Has.Count.EqualTo(9));

            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEFAULT_SLOT_VALUE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(3 * generated.Slots.Count));
            Assert.That(response.Value, Is.EqualTo(generated.Slots.Select(s => new RDMDefaultSlotValue(s.Value.SlotId, s.Value.DefaultValue))));
            #endregion

            #region Test Invalid Calls
            Assert.Throws(typeof(NullReferenceException), () => generated.CurrentPersonality = null); // Change to personality null
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 0); // Change to personality 0
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 4); // Change to personality 4
            Assert.DoesNotThrow(() => generated.CurrentPersonality = 3); // Change to personality 3
            #endregion
        }

        [Test, Order(42)]
        public void TestGetSLOT_DESCRIPTION()
        {
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SLOT_DESCRIPTION,
                SubDevice = SubDevice.Root,
            };
            RDMMessage? response = null;
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Slots, Has.Count.EqualTo(5));
            doTests(generated.Slots.Values.ToArray());
            #endregion

            #region Test Change Personality 2
            generated.CurrentPersonality = 2; // Change to personality 2
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Slots, Has.Count.EqualTo(8));
            doTests(generated.Slots.Values.ToArray());
            #endregion

            #region Test Change Personality 3
            generated.CurrentPersonality = 3; // Change to personality 3
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Slots, Has.Count.EqualTo(9));
            doTests(generated.Slots.Values.ToArray());
            #endregion

            #region Test Invalid Calls
            Assert.Throws(typeof(NullReferenceException), () => generated.CurrentPersonality = null); // Change to personality null
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 0); // Change to personality 0
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => generated.CurrentPersonality = 4); // Change to personality 4
            Assert.DoesNotThrow(() => generated.CurrentPersonality = 3); // Change to personality 3

            request.ParameterData = new byte[] { (byte)((10 >> 8) & 0xFF), (byte)(10 & 0xFF) };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.ACTION_NOT_SUPPORTED }));
            #endregion

            void doTests(Slot[] slots)
            {
                foreach(Slot slot in slots)
                {
                    request.ParameterData = new byte[] { (byte)((slot.SlotId >> 8) & 0xFF), (byte)(slot.SlotId & 0xFF) };
                    response = generated.ProcessRequestMessage_Internal(request);
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(2 + slot.Description.Length));
                    Assert.That(response.Value, Is.EqualTo(new RDMSlotDescription(slot.SlotId, slot.Description)));
                }
            }
        }

        [Test, Order(51)]
        public void TestGetSENSOR_DEFINITION()
        {
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SENSOR_DEFINITION,
                SubDevice = SubDevice.Root,
            };
            RDMMessage? response = null;
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));
            doTests(generated.Sensors.Values.ToArray());
            #endregion

            void doTests(Sensor[] sensors)
            {
                foreach (Sensor sensor in sensors)
                {
                    request.ParameterData = new byte[] { sensor.SensorId };
                    response = generated.ProcessRequestMessage_Internal(request);
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(13 + sensor.Description.Length));
                    Assert.That(response.Value, Is.EqualTo(new RDMSensorDefinition(sensor.SensorId, sensor.Type, sensor.Unit, sensor.Prefix, sensor.RangeMinimum, sensor.RangeMaximum, sensor.NormalMinimum, sensor.NormalMaximum, sensor.LowestHighestValueSupported, sensor.RecordedValueSupported, sensor.Description)));
                }
            }
        }

        [Test, Order(52)]
        public void TestGetSENSOR_VALUE()
        {
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SENSOR_VALUE,
                SubDevice = SubDevice.Root,
            };
            RDMMessage? response = null;
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));
            doTests(generated.Sensors.Values.ToArray());
            #endregion

            void doTests(Sensor[] sensors)
            {
                foreach (Sensor sensor in sensors)
                {
                    request.ParameterData = new byte[] { sensor.SensorId };
                    response = generated.ProcessRequestMessage_Internal(request);
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                    Assert.That(response.Value, Is.EqualTo(new RDMSensorValue(sensor.SensorId, sensor.PresentValue, sensor.LowestValue, sensor.HighestValue, sensor.RecordedValue)));
                }
            }
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
            Assert.That(statusMessages[0].StatusType, Is.EqualTo(ERDM_Status.ERROR_CLEARED));
            for (int i = 0; i < 2; i++)
                Assert.That(statusMessages[i], Is.EqualTo(generated.StatusMessages[i]));
            #endregion
        }
        [Test, Order(1000)]
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
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_DEFINITION));
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