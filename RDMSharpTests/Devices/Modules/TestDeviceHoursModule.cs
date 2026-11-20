using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestDeviceHoursModule
{
    private DeviceHoursMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.DEVICE_HOURS;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public void Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new DeviceHoursMockDevice(DEVCIE_UID);
    }
    [TearDown]
    public void TearDown()
    {
        generated?.Dispose();
        generated = null;
    }

    [Test]
    public void TestDisplayName()
    {
        Assert.That(generated, Is.Not.Null);
        var deviceHoursModule = generated.Modules.OfType<DeviceHoursModule>().Single();
        Assert.That(deviceHoursModule, Is.Not.Null);
        Assert.That(deviceHoursModule.DisplayName, Is.EqualTo("Device Hours"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetDEVICE_HOURS()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);

        var module = generated.Modules.OfType<DeviceHoursModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DeviceHours, Is.Zero);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.DEVICE_HOURS,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(module.DeviceHours));
            Assert.That(module.DeviceHours, Is.Zero);
        }
        #endregion

        #region Test Device Hours changed (GET)
        module.DeviceHours = 1;
        Assert.That(module.DeviceHours, Is.EqualTo(1));
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(module.DeviceHours));
            Assert.That(module.DeviceHours, Is.EqualTo(1));
        }
        #endregion

        #region Test Device Hours changed (SET)
        request.ParameterData = Tools.ValueToData((uint)0x0990);
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.DeviceHours, Is.EqualTo(0x0990));
        }

        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(module.DeviceHours));
        }

        request.ParameterData = Tools.ValueToData((uint)0x9009);
        request.Command = ERDM_Command.SET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            Assert.That(module.DeviceHours, Is.EqualTo(0x9009));
        }

        request.Command = ERDM_Command.GET_COMMAND;
        response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.DEVICE_HOURS));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(module.DeviceHours));
        }

        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetDEVICE_HOURS))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DeviceHoursModule>().Single();
        generatedModule.DeviceHours = 10000;
        Assert.That(generatedModule, Is.Not.Null);
        Assert.That(generatedModule.DeviceHours, Is.EqualTo(10000));

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DeviceHoursModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DeviceHours, Is.EqualTo(10000));

        await module.SetDeviceHours(0x1234);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.DeviceHours, Is.EqualTo(0x1234))
            && Wait.Holds(module.DeviceHours, Is.EqualTo(0x1234)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.DeviceHours, Is.EqualTo(0x1234));
            Assert.That(module.DeviceHours, Is.EqualTo(0x1234));
        }
    }
    class DeviceHoursMockDevice : MockGeneratedDevice1
    {
        public DeviceHoursMockDevice(UID uid) : base(uid, new IModule[] { new DeviceHoursModule() })
        {
        }
    }
}