using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// DISPLAY_LEVEL (E1.20 10.9.2): GET/SET of the display intensity,
/// 0x00 = display off, 0xFF = full, values in between are a relative intensity.
/// </summary>
public class TestDisplayLevelModule
{
    private DisplayLevelMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.DISPLAY_LEVEL;
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
        generated = new DisplayLevelMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<DisplayLevelModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Display Level"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetDISPLAY_LEVEL()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<DisplayLevelModule>().Single();

        // the display is on full by default
        Assert.That(module.DisplayLevel, Is.EqualTo(0xFF));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 0xFF);

        module.DisplayLevel = 0x40;
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 0x40);

        foreach (byte level in new byte[] { 0x00, 0x80, 0xFF })
        {
            var request = createRequest(ERDM_Command.SET_COMMAND);
            request.ParameterData = new byte[] { level };
            RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
            Assert.That(response, Is.Not.Null);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
                Assert.That(module.DisplayLevel, Is.EqualTo(level));
            }
            assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), level);
        }

        // after a SET by RDM the device sets a value locally again, which was its value before the SET
        module.DisplayLevel = 0x40;
        var setRequest = createRequest(ERDM_Command.SET_COMMAND);
        setRequest.ParameterData = new byte[] { 0x10 };
        generated.ProcessRequestMessage_Internal(setRequest);
        Assert.That(module.DisplayLevel, Is.EqualTo(0x10));
        module.DisplayLevel = 0x40;
        Assert.That(module.DisplayLevel, Is.EqualTo(0x40), "setting the value locally after a SET must not be ignored");
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 0x40);
    }

    [Test, DependsOnTest(nameof(TestGetSetDISPLAY_LEVEL))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<DisplayLevelModule>().Single();
        generatedModule.DisplayLevel = 0x20;

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<DisplayLevelModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.DisplayLevel, Is.EqualTo(0x20)));
        Assert.That(module.DisplayLevel, Is.EqualTo(0x20));

        await module.SetDisplayLevel(0x00);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.DisplayLevel, Is.Zero)
            && Wait.Holds(module.DisplayLevel, Is.Zero));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.DisplayLevel, Is.Zero);
            Assert.That(module.DisplayLevel, Is.Zero);
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
    private static void assertGetResponse(RDMMessage? response, byte expected)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(new byte[] { expected }));
        }
    }

    class DisplayLevelMockDevice : MockGeneratedDevice1
    {
        public DisplayLevelMockDevice(UID uid) : base(uid, new IModule[] { new DisplayLevelModule() })
        {
        }
    }
}
