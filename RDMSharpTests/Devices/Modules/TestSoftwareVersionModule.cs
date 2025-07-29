using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules
{
    public class TestSoftwareVersionModule
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
    }
}