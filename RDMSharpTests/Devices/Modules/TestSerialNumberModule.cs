using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// SERIAL_NUMBER (E1.37-5 4.7): GET only, a string of up to 231 bytes.
/// A longer serial has to be truncated by the manufacturer, so the module rejects it.
/// </summary>
public class TestSerialNumberModule
{
    private SerialNumberMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.SERIAL_NUMBER;
    private static readonly UID CONTROLLER_UID = new UID(0x1fff + (ushort)MODULE_PARAMETER, 333);
    private static readonly UID DEVCIE_UID = new UID(123 + (ushort)MODULE_PARAMETER, 555);
    private const string SERIAL = "SN-2024-000815";

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
        generated = new SerialNumberMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<SerialNumberModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Serial Number"));
    }

    [Test]
    public void TestMaximumLength()
    {
        Assert.DoesNotThrow(() => new SerialNumberModule(new string('A', 231)));
        Assert.Throws<ArgumentException>(() => new SerialNumberModule(new string('A', 232)));
        // the limit is in bytes, a multi byte character counts several times
        Assert.Throws<ArgumentException>(() => new SerialNumberModule(new string('Ä', 116)));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSERIAL_NUMBER()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<SerialNumberModule>().Single();
        Assert.That(module.SerialNumber, Is.EqualTo(SERIAL));

        RDMMessage? response = generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND));
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(SERIAL.Length));
            Assert.That(response.Value, Is.EqualTo(SERIAL));
        }
    }

    [Test, DependsOnTest(nameof(TestGetSERIAL_NUMBER))]
    public void TestSetSERIAL_NUMBER()
    {
        Assert.That(generated, Is.Not.Null);
        var request = createRequest(ERDM_Command.SET_COMMAND);
        request.ParameterData = Tools.ValueToData("SN-OTHER");

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
            Assert.That(generated.Modules.OfType<SerialNumberModule>().Single().SerialNumber, Is.EqualTo(SERIAL));
        }
    }

    [Test, DependsOnTest(nameof(TestGetSERIAL_NUMBER))]
    public async Task TestRemoteDevice()
    {
        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<SerialNumberModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.SerialNumber, Is.EqualTo(SERIAL)));
        Assert.That(module.SerialNumber, Is.EqualTo(SERIAL));
    }

    [Test, DependsOnTest(nameof(TestRemoteDevice))]
    public async Task TestRemoteDeviceSerialPerDevice()
    {
        // The serial number belongs to the single device, not to the model:
        // a second device of the same model (same manufacturer, model and software) has its own serial.
        const string OTHER_SERIAL = "SN-2024-000816";
        UID otherUid = new UID(DEVCIE_UID.ManufacturerID, DEVCIE_UID.DeviceID + 1);
        using var otherDevice = new SerialNumberMockDevice(otherUid, OTHER_SERIAL);

        using MockDevice remote = new MockDevice(DEVCIE_UID);
        using MockDevice otherRemote = new MockDevice(otherUid);
        while (!remote.IsInitialized || !otherRemote.IsInitialized)
            await Task.Delay(100);

        var module = remote.Modules.OfType<SerialNumberModule>().Single();
        var otherModule = otherRemote.Modules.OfType<SerialNumberModule>().Single();
        await Wait.UntilAsync(() =>
            Wait.Holds(module.SerialNumber, Is.EqualTo(SERIAL))
            && Wait.Holds(otherModule.SerialNumber, Is.EqualTo(OTHER_SERIAL)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.SerialNumber, Is.EqualTo(SERIAL));
            Assert.That(otherModule.SerialNumber, Is.EqualTo(OTHER_SERIAL));
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

    class SerialNumberMockDevice : MockGeneratedDevice1
    {
        public SerialNumberMockDevice(UID uid, string serial = SERIAL) : base(uid, new IModule[] { new SerialNumberModule(serial) })
        {
        }
    }
}
