using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestBootSoftwareVersionModule
{
    private MockGeneratedDevice1? generated;

    private static UID CONTROLLER_UID = new UID(0x1fff, 333);
    private static UID DEVCIE_UID = new UID(123, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

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
}