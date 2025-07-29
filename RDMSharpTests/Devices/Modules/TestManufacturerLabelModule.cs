using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules
{
    public class TestManufacturerLabelModule
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
    }
}