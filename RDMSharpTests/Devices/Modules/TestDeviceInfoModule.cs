using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestDeviceInfoModule
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

    [Test, Order(11)]
    public async Task TestRemoteDevice()
    {
        MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DeviceInfoModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DeviceInfo, Is.Not.Null);
        Assert.That(module.DeviceInfo.SensorCount, Is.EqualTo(5));
    }
}