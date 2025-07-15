using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
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
            var defines = MetadataFactory.GetMetadataDefineVersions();
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(generated.Parameters.Count * 2));
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
            RDMDeviceInfo? deviceInfo = response!.Value as RDMDeviceInfo;
            Assert.That(deviceInfo, Is.Not.Null);
            Assert.That(deviceInfo.SensorCount, Is.EqualTo(5));
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

            #region Test Identify changed (GET)
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


            #region Test Identify changed (SET)
            Assert.That(generated.Identify, Is.True);
            request.ParameterData = new byte[] { 0x00 }; // Requesting Identify OFF
            request.Command = ERDM_Command.SET_COMMAND;
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));

            Assert.That(generated.Identify, Is.False);
            request.Command = ERDM_Command.GET_COMMAND;
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

            request.ParameterData = new byte[] { 0x01 }; // Requesting Identify OFF
            request.Command = ERDM_Command.SET_COMMAND;
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.IDENTIFY_DEVICE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));

            Assert.That(generated.Identify, Is.True);

            request.Command = ERDM_Command.GET_COMMAND;
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
            var deviceLabelModule = generated.Modules.OfType<DeviceLabelModule>().Single();
            Assert.That(deviceLabelModule, Is.Not.Null);
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("Dummy Device 1"));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(deviceLabelModule.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(deviceLabelModule.DeviceLabel));
            #endregion

            #region Test Label changed
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("Dummy Device 1"));
            deviceLabelModule.DeviceLabel = "Rem x Ram";
            Assert.That(deviceLabelModule.DeviceLabel, Is.EqualTo("Rem x Ram"));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(deviceLabelModule.DeviceLabel.Length));
            Assert.That(response.Value, Is.EqualTo(deviceLabelModule.DeviceLabel));
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
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.DATA_OUT_OF_RANGE }));

            for (byte b = 0; b < generated.Personalities.Count; b++)
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
                var pers = generated.Personalities.ElementAt(b);
                var expected = new RDMDMXPersonalityDescription(pers.ID, pers.SlotCount, pers.Description);
                Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
                Assert.That(response.Value, Is.EqualTo(expected));
                Assert.That(((RDMDMXPersonalityDescription)response.Value).Index, Is.EqualTo(expected.Index));
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
            var pers = generated.Personalities.ElementAt(0);
            var expected = new RDMDMXPersonality(pers.ID, (byte)generated.Personalities.Count());
            Assert.That(response.ParameterData, Has.Length.EqualTo(expected.ToPayloadData().Length));
            Assert.That(response.Value, Is.EqualTo(expected));
            Assert.That(((RDMDMXPersonality)response.Value).MinIndex, Is.EqualTo(1));
            Assert.That(((RDMDMXPersonality)response.Value).Index, Is.EqualTo(1));
            Assert.That(((RDMDMXPersonality)response.Value).Count, Is.EqualTo(3));
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
            pers = generated.Personalities.ElementAt(1);
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
            var manufacturerLabelModule = generated.Modules.OfType<ManufacturerLabelModule>().Single();
            Assert.That(manufacturerLabelModule, Is.Not.Null);
            Assert.That(manufacturerLabelModule.ManufacturerLabel, Is.EqualTo("Dummy Manufacturer 9FFF"));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(manufacturerLabelModule.ManufacturerLabel.Length));
            Assert.That(response.Value, Is.EqualTo(manufacturerLabelModule.ManufacturerLabel));
            #endregion
        }
        [Test, Order(16)]
        public void TestGetDEVICE_MODEL_DESCRIPTION()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            var deviceModelDescriptionModule = generated.Modules.OfType<DeviceModelDescriptionModule>().Single();
            Assert.That(deviceModelDescriptionModule, Is.Not.Null);
            Assert.That(deviceModelDescriptionModule.DeviceModelDescription, Is.EqualTo("Test Model Description"));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(deviceModelDescriptionModule.DeviceModelDescription.Length));
            Assert.That(response.Value, Is.EqualTo(deviceModelDescriptionModule.DeviceModelDescription));
            #endregion
        }
        [Test, Order(30)]
        public void TestGetBOOT_SOFTWARE_VERSION_ID()
        {
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            var bootSoftwareVersionModule = generated.Modules.OfType<BootSoftwareVersionModule>().Single();
            Assert.That(bootSoftwareVersionModule, Is.Not.Null);
            Assert.That(bootSoftwareVersionModule.BootSoftwareVersionId, Is.EqualTo(123));
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
            Assert.That(response.Value, Is.EqualTo(bootSoftwareVersionModule.BootSoftwareVersionId));
            #endregion
        }
        [Test, Order(31)]
        public void TestGetBOOT_SOFTWARE_VERSION_LABEL()
        {
            const string BOOT_SOFTWARE_VERSION_LABEL = "Dummy Bootloader Software";
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            var bootSoftwareVersionModule = generated.Modules.OfType<BootSoftwareVersionModule>().Single();
            Assert.That(bootSoftwareVersionModule, Is.Not.Null);
            Assert.That(bootSoftwareVersionModule.BootSoftwareVersionLabel, Is.EqualTo(BOOT_SOFTWARE_VERSION_LABEL));
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
            Assert.That(response.ParameterData, Has.Length.EqualTo(BOOT_SOFTWARE_VERSION_LABEL.Length));
            Assert.That(response.Value, Is.EqualTo(BOOT_SOFTWARE_VERSION_LABEL));
            #endregion

            #region Test Label changed
            bootSoftwareVersionModule.BootSoftwareVersionLabel = "Rem x Ram";
            Assert.That(bootSoftwareVersionModule.BootSoftwareVersionLabel, Is.EqualTo("Rem x Ram"));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.BOOT_SOFTWARE_VERSION_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(bootSoftwareVersionModule.BootSoftwareVersionLabel.Length));
            Assert.That(response.Value, Is.EqualTo(bootSoftwareVersionModule.BootSoftwareVersionLabel));
            #endregion
        }
        [Test, Order(32)]
        public void TestGetSOFTWARE_VERSION_LABEL()
        {
            const string SOFTWARE_VERSION_LABEL = "Dummy Software";
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            var softwareVersionModule = generated.Modules.OfType<SoftwareVersionModule>().Single();
            Assert.That(softwareVersionModule, Is.Not.Null);
            Assert.That(softwareVersionModule.SoftwareVersionLabel, Is.EqualTo(SOFTWARE_VERSION_LABEL));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.SOFTWARE_VERSION_LABEL,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SOFTWARE_VERSION_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(SOFTWARE_VERSION_LABEL.Length));
            Assert.That(response.Value, Is.EqualTo(SOFTWARE_VERSION_LABEL));
            #endregion

            #region Test Label changed
            softwareVersionModule.SoftwareVersionLabel = "Rem x Ram";
            Assert.That(softwareVersionModule.SoftwareVersionLabel, Is.EqualTo("Rem x Ram"));
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SOFTWARE_VERSION_LABEL));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(softwareVersionModule.SoftwareVersionLabel.Length));
            Assert.That(response.Value, Is.EqualTo(softwareVersionModule.SoftwareVersionLabel));
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
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.DATA_OUT_OF_RANGE }));
            #endregion

            void doTests(Slot[] slots)
            {
                foreach (Slot slot in slots)
                {
                    request.ParameterData = new byte[] { (byte)((slot.SlotId >> 8) & 0xFF), (byte)(slot.SlotId & 0xFF) };
                    response = generated.ProcessRequestMessage_Internal(request);
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SLOT_DESCRIPTION));
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
            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors, Has.Count.EqualTo(5));

                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(12000));

                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));

                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(0));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(0));
            });

            doTests(generated.Sensors.Values.ToArray());
            #endregion

            #region Test New Sensor Values
            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));
            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(99);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(122);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(155);

            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(99));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(122));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(155));

                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(99));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(122));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(155));

                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
            });
            doTests(generated.Sensors.Values.ToArray());
            #endregion

            #region Test Recorded Sensor Values
            Assert.Multiple(() =>
            {
                Assert.That(generated, Is.Not.Null);
                Assert.That(generated.Sensors, Has.Count.EqualTo(5));
                Assert.That(generated.Sensors[0].RecordedValueSupported, Is.True);
                Assert.That(generated.Sensors[1].RecordedValueSupported, Is.True);
                Assert.That(generated.Sensors[2].RecordedValueSupported, Is.True);
                Assert.That(generated.Sensors[0].LowestHighestValueSupported, Is.True);
                Assert.That(generated.Sensors[1].LowestHighestValueSupported, Is.True);
                Assert.That(generated.Sensors[2].LowestHighestValueSupported, Is.True);
            });

            ((MockGeneratedSensor)generated.Sensors[0]).RecordValue();
            ((MockGeneratedSensor)generated.Sensors[1]).RecordValue();
            ((MockGeneratedSensor)generated.Sensors[2]).RecordValue();

            ((MockGeneratedSensor)generated.Sensors[0]).UpdateValue(111);
            ((MockGeneratedSensor)generated.Sensors[1]).UpdateValue(666);
            ((MockGeneratedSensor)generated.Sensors[2]).UpdateValue(987);

            Assert.Multiple(() =>
            {
                Assert.That(generated.Sensors[0].RecordedValue, Is.EqualTo(99));
                Assert.That(generated.Sensors[1].RecordedValue, Is.EqualTo(122));
                Assert.That(generated.Sensors[2].RecordedValue, Is.EqualTo(155));

                Assert.That(generated.Sensors[0].PresentValue, Is.EqualTo(111));
                Assert.That(generated.Sensors[1].PresentValue, Is.EqualTo(666));
                Assert.That(generated.Sensors[2].PresentValue, Is.EqualTo(987));

                Assert.That(generated.Sensors[0].LowestValue, Is.EqualTo(99));
                Assert.That(generated.Sensors[1].LowestValue, Is.EqualTo(122));
                Assert.That(generated.Sensors[2].LowestValue, Is.EqualTo(155));

                Assert.That(generated.Sensors[0].HighestValue, Is.EqualTo(3000));
                Assert.That(generated.Sensors[1].HighestValue, Is.EqualTo(8000));
                Assert.That(generated.Sensors[2].HighestValue, Is.EqualTo(12000));
            });
            doTests(generated.Sensors.Values.ToArray());
            #endregion

            #region Test Reset Sensor Values Set_Request
            request.Command = ERDM_Command.SET_COMMAND;
            for (byte b = 0; b < 2; b++)
            {
                request.ParameterData = new byte[] { b };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                    Assert.That(response.ParameterData[0], Is.EqualTo(b));
                    Assert.That(response.Value, Is.TypeOf(typeof(RDMSensorValue)));
                });
            }
            #endregion

            #region Test Reset Sensor Values Set_Request (Broadcast)
            request.ParameterData = new byte[] { 0xff };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                Assert.That(response.ParameterData[0], Is.EqualTo(0xff));
                Assert.That(response.Value, Is.TypeOf(typeof(RDMSensorValue)));
            });
            foreach (var generatedSensor in generated.Sensors.Values)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(generatedSensor, Is.Not.Null);
                    Sensor remoteSensor = generated.Sensors[generatedSensor.SensorId];
                    Assert.That(remoteSensor, Is.Not.Null);
                    Assert.That(remoteSensor.PresentValue, Is.EqualTo(generatedSensor.PresentValue));
                    Assert.That(remoteSensor.LowestValue, Is.EqualTo(generatedSensor.LowestValue));
                    Assert.That(remoteSensor.HighestValue, Is.EqualTo(generatedSensor.HighestValue));
                    Assert.That(remoteSensor.RecordedValue, Is.EqualTo(generatedSensor.RecordedValue));
                });
            }
            #endregion

            void doTests(Sensor[] sensors)
            {
                foreach (Sensor sensor in sensors)
                {
                    request.ParameterData = new byte[] { sensor.SensorId };
                    response = generated.ProcessRequestMessage_Internal(request);
                    Assert.Multiple(() =>
                    {
                        Assert.That(response, Is.Not.Null);
                        Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
                        Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                        Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                        Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.SENSOR_VALUE));
                        Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                        Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                        Assert.That(response.ParameterData, Has.Length.EqualTo(9));
                        Assert.That(response.Value, Is.EqualTo(new RDMSensorValue(sensor.SensorId, sensor.PresentValue, sensor.LowestValue, sensor.HighestValue, sensor.RecordedValue)));
                    });
                }
            }
        }
        [Test, Order(53)]
        public void TestGetRECORD_SENSORS()
        {
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.SET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.RECORD_SENSORS,
                SubDevice = SubDevice.Root,
            };
            RDMMessage? response = null;

            Assert.That(generated, Is.Not.Null);
            Assert.That(generated.Sensors, Has.Count.EqualTo(5));

            #region Test Recorded Sensor Values Set_Request
            for (byte b = 0; b < 2; b++)
            {
                request.ParameterData = new byte[] { b };
                response = generated.ProcessRequestMessage_Internal(request);
                Assert.That(response, Is.Not.Null);
                Assert.Multiple(() =>
                {
                    Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                    Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                    Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                    Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
                    Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                    Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                    Assert.That(response.ParameterData, Has.Length.EqualTo(0));
                });
            }
            #endregion

            #region Test Recorded Sensor Values Set_Request (Broadcast)
            request.ParameterData = new byte[] { 0xff };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            });
            foreach (var generatedSensor in generated.Sensors.Values)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(generatedSensor, Is.Not.Null);
                    Sensor remoteSensor = generated.Sensors[generatedSensor.SensorId];
                    Assert.That(remoteSensor, Is.Not.Null);
                    Assert.That(remoteSensor.PresentValue, Is.EqualTo(generatedSensor.PresentValue));
                    Assert.That(remoteSensor.LowestValue, Is.EqualTo(generatedSensor.LowestValue));
                    Assert.That(remoteSensor.HighestValue, Is.EqualTo(generatedSensor.HighestValue));
                    Assert.That(remoteSensor.RecordedValue, Is.EqualTo(generatedSensor.RecordedValue));
                });
            }
            #endregion

            #region Test Invalid Calls
            request.ParameterData = new byte[] { 30 };
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.DATA_OUT_OF_RANGE }));

            request.ParameterData = new byte[] { 2 };
            request.Command = ERDM_Command.GET_COMMAND;
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.RECORD_SENSORS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(new ERDM_NackReason[] { ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS }));
            #endregion
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

        [Test, Retry(3), Order(61)]
        public async Task TestGetREAL_TIME_CLOCK()
        {
            TearDown();
            generated = new RealTimeMockDevice(DEVCIE_UID);
            await Task.Delay(500);
            #region Test Basic
            Assert.That(generated, Is.Not.Null);
            await Task.Delay(1000);
            var realTimeClockModule = generated.Modules.OfType<RealTimeClockModule>().FirstOrDefault();
            Assert.That(realTimeClockModule, Is.Not.Null);
            Assert.That(realTimeClockModule.RealTimeClock, Is.Not.Null);
            Assert.That(realTimeClockModule.RealTimeClock.Value.Minute, Is.EqualTo(DateTime.Now.Minute));
            RDMMessage request = new RDMMessage()
            {
                Command = ERDM_Command.GET_COMMAND,
                DestUID = DEVCIE_UID,
                SourceUID = CONTROLLER_UID,
                Parameter = ERDM_Parameter.REAL_TIME_CLOCK,
                SubDevice = SubDevice.Root,
            };

            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.REAL_TIME_CLOCK));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(7));
            Assert.That(response.Value, Is.TypeOf(typeof(RDMRealTimeClock)));
            var timeGen = new RDMRealTimeClock(realTimeClockModule.RealTimeClock.Value);
            var timeRem = (RDMRealTimeClock)response.Value;
            Assert.That(timeRem.Year, Is.EqualTo(timeGen.Year));
            Assert.That(timeRem.Month, Is.EqualTo(timeGen.Month));
            Assert.That(timeRem.Day, Is.EqualTo(timeGen.Day));
            Assert.That(timeRem.Minute, Is.EqualTo(timeGen.Minute));
            Assert.That(timeRem.Second, Is.AtLeast(timeGen.Second - 2).And.AtMost(timeGen.Second + 2));
            #endregion

        }
        class RealTimeMockDevice : MockGeneratedDevice1
        {
            public RealTimeMockDevice(UID uid) : base(uid, new IModule[] { new RealTimeClockModule() })
            {
            }
        }
    }
}