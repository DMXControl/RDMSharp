using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// IDENTIFY_MODE (E1.37-1 3.2): GET/SET of the identify mode,
/// 0x00 = Quiet Identify, 0xFF = Loud Identify, 0x01-0xFE are not defined.
/// </summary>
public class TestIdentifyModeModule
{
    private IdentifyModeMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.IDENTIFY_MODE;
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
        generated = new IdentifyModeMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<IdentifyModeModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Identify Mode"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetIDENTIFY_MODE()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<IdentifyModeModule>().Single();

        #region Test Basic: quiet by default
        Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), ERDM_IdentifyMode.QUIET);
        #endregion

        #region Test SET loud and quiet
        assertSetResponse(send(0xFF), null);
        Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.LOUD));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), ERDM_IdentifyMode.LOUD);

        assertSetResponse(send(0x00), null);
        Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET));

        // after a SET by RDM the device sets the mode locally again, which was its value before the SET
        assertSetResponse(send(0xFF), null);
        module.IdentifyMode = ERDM_IdentifyMode.QUIET;
        Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET), "setting the mode locally after a SET must not be ignored");
        #endregion

        #region Test undefined values 0x01-0xFE
        foreach (byte undefined in new byte[] { 0x01, 0x7F, 0xFE })
        {
            assertSetResponse(send(undefined), ERDM_NackReason.DATA_OUT_OF_RANGE);
            Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET), "rejected request must not change the mode");
        }
        #endregion

        #region Test invalid PDL
        foreach (byte[] data in new[] { Array.Empty<byte>(), new byte[] { 0xFF, 0xFF } })
        {
            var request = createRequest(ERDM_Command.SET_COMMAND);
            request.ParameterData = data;
            assertSetResponse(generated.ProcessRequestMessage_Internal(request), ERDM_NackReason.FORMAT_ERROR);
            Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET), "rejected request must not change the mode");
        }
        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetSetIDENTIFY_MODE))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<IdentifyModeModule>().Single();

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<IdentifyModeModule>().Single();
        await Wait.UntilAsync(() => Wait.Holds(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET)));
        Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.QUIET));

        await module.SetIdentifyMode(ERDM_IdentifyMode.LOUD);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.LOUD))
            && Wait.Holds(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.LOUD)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.LOUD));
            Assert.That(module.IdentifyMode, Is.EqualTo(ERDM_IdentifyMode.LOUD));
        }
    }

    private RDMMessage? send(byte value)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND);
        request.ParameterData = new byte[] { value };
        return generated!.ProcessRequestMessage_Internal(request);
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
    private static void assertGetResponse(RDMMessage? response, ERDM_IdentifyMode expected)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(new byte[] { (byte)expected }));
            Assert.That(response.Value, Is.EqualTo(expected));
        }
    }
    private static void assertSetResponse(RDMMessage? response, ERDM_NackReason? nackReason)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            if (nackReason is null)
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
                Assert.That(response.ParameterData, Has.Length.EqualTo(0));
            }
            else
            {
                Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.NACK_REASON));
                Assert.That(response.NackReason, Is.EqualTo(nackReason));
            }
        }
    }

    class IdentifyModeMockDevice : MockGeneratedDevice1
    {
        public IdentifyModeMockDevice(UID uid) : base(uid, new IModule[] { new IdentifyModeModule() })
        {
        }
    }
}
