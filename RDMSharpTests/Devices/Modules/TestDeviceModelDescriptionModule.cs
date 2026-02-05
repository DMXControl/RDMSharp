using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestDeviceModelDescriptionModule
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
}