using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// DEVICE_UNIT_NUMBER (E1.37-5 4.15): GET/SET of a 32-bit unit number.
/// Valid unit numbers are 0x00000001 - 0xFFFFFFFF, 0x00000000 means "un-set".
/// </summary>
public class TestDeviceUnitNumberModule
{
    private DeviceUnitNumberMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.DEVICE_UNIT_NUMBER;
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
        generated = new DeviceUnitNumberMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<DeviceUnitNumberModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Device Unit Number"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetDEVICE_UNIT_NUMBER()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<DeviceUnitNumberModule>().Single();

        #region Test Basic: un-set by default
        Assert.That(module.DeviceUnitNumber, Is.Zero);
        RDMMessage request = createRequest(ERDM_Command.GET_COMMAND);
        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        assertGetResponse(response, 0);
        #endregion

        #region Test changed locally (GET)
        module.DeviceUnitNumber = 42;
        Assert.That(module.DeviceUnitNumber, Is.EqualTo(42));
        response = generated.ProcessRequestMessage_Internal(request);
        assertGetResponse(response, 42);
        #endregion

        #region Test changed by SET
        foreach (uint unitNumber in new uint[] { 0x12345678, uint.MaxValue, 1, 0 })
        {
            request = createRequest(ERDM_Command.SET_COMMAND);
            request.ParameterData = Tools.ValueToData(unitNumber);
            response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
                Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
                Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
                Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
                Assert.That(module.DeviceUnitNumber, Is.EqualTo(unitNumber), $"Unit Number {unitNumber}");
            }

            response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND));
            assertGetResponse(response, unitNumber);
        }
        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetDEVICE_UNIT_NUMBER))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DeviceUnitNumberModule>().Single();
        generatedModule.DeviceUnitNumber = 7;

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DeviceUnitNumberModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.DeviceUnitNumber, Is.EqualTo(7)));
        Assert.That(module.DeviceUnitNumber, Is.EqualTo(7));

        await module.SetDeviceUnitNumber(0xCAFE);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.DeviceUnitNumber, Is.EqualTo(0xCAFE))
            && Wait.Holds(module.DeviceUnitNumber, Is.EqualTo(0xCAFE)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.DeviceUnitNumber, Is.EqualTo(0xCAFE));
            Assert.That(module.DeviceUnitNumber, Is.EqualTo(0xCAFE));
        }
    }

    private static RDMMessage createRequest(ERDM_Command command)
    {
        return new RDMMessage()
        {
            Command = command,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = MODULE_PARAMETER,
            SubDevice = SubDevice.Root,
        };
    }
    private static void assertGetResponse(RDMMessage? response, uint expected)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(4));
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }

    class DeviceUnitNumberMockDevice : MockGeneratedDevice1
    {
        public DeviceUnitNumberMockDevice(UID uid) : base(uid, new IModule[] { new DeviceUnitNumberModule() })
        {
        }
    }
}
