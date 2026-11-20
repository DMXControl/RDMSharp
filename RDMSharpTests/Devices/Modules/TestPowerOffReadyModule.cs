using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// POWER_OFF_READY (E1.37-5 4.6): GET only, 0x01 if power can be removed from the device, otherwise 0x00.
/// A device that can still respond after its shutdown should queue a POWER_OFF_READY message,
/// so a controller learns about the change without polling.
/// </summary>
public class TestPowerOffReadyModule
{
    private PowerOffReadyMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.POWER_OFF_READY;
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
        generated = new PowerOffReadyMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<PowerOffReadyModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Power Off Ready"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetPOWER_OFF_READY()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<PowerOffReadyModule>().Single();

        Assert.That(module.PowerOffReady, Is.False);
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), false);

        module.PowerOffReady = true;
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), true);

        module.PowerOffReady = false;
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), false);
    }

    [Test, DependsOnTest(nameof(TestGetPOWER_OFF_READY))]
    public void TestSetPOWER_OFF_READY()
    {
        Assert.That(generated, Is.Not.Null);
        var request = createRequest(ERDM_Command.SET_COMMAND);
        // POWER_OFF_READY is GET only, there is no SET payload the parser could build
        request.ParameterData = new byte[] { 0x01 };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
            Assert.That(response.NackReason, Is.EqualTo(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS));
            Assert.That(generated.Modules.OfType<PowerOffReadyModule>().Single().PowerOffReady, Is.False);
        }
    }

    [Test, DependsOnTest(nameof(TestGetPOWER_OFF_READY))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<PowerOffReadyModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<PowerOffReadyModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.PowerOffReady, Is.False));
        Assert.That(module.PowerOffReady, Is.False);

        // the shutdown is finished, the remote device learns it via the queued messages
        generatedModule.PowerOffReady = true;
        await Wait.UntilAsync(() => Wait.Holds(module.PowerOffReady, Is.True));
        Assert.That(module.PowerOffReady, Is.True);
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
    private static void assertGetResponse(RDMMessage? response, bool expected)
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
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }

    class PowerOffReadyMockDevice : MockGeneratedDevice1
    {
        public PowerOffReadyMockDevice(UID uid) : base(uid, new IModule[] { new PowerOffReadyModule() })
        {
        }
    }
}
