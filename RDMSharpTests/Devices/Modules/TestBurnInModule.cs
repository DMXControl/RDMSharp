using RDMSharp.Metadata;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

/// <summary>
/// BURN_IN (E1.37-1 3.10): GET returns the remaining burn-in time in hours (0-255),
/// SET sets the burn-in time and starts the burn-in, SET 0 aborts a running burn-in.
/// </summary>
public class TestBurnInModule
{
    private BurnInMockDevice? generated;

    private const ERDM_Parameter MODULE_PARAMETER = ERDM_Parameter.BURN_IN;
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
        generated = new BurnInMockDevice(DEVCIE_UID);
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
        var module = generated.Modules.OfType<BurnInModule>().Single();
        Assert.That(module, Is.Not.Null);
        Assert.That(module.DisplayName, Is.EqualTo("Burn-In"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public void TestGetSetBURN_IN()
    {
        Assert.That(generated, Is.Not.Null);
        var module = generated.Modules.OfType<BurnInModule>().Single();

        #region Test Basic: no burn-in running
        Assert.That(module.HoursRemaining, Is.Zero);
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 0);
        #endregion

        #region Test start burn-in (SET)
        assertSetAck(generated.ProcessRequestMessage_Internal(createSetRequest(24)));
        Assert.That(module.HoursRemaining, Is.EqualTo(24));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 24);
        #endregion

        #region Test the device counts down the remaining time
        module.HoursRemaining = 23;
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 23);
        #endregion

        #region Test maximum (255 hours)
        assertSetAck(generated.ProcessRequestMessage_Internal(createSetRequest(255)));
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 255);
        #endregion

        #region Test abort burn-in (SET 0)
        assertSetAck(generated.ProcessRequestMessage_Internal(createSetRequest(0)));
        Assert.That(module.HoursRemaining, Is.Zero);
        assertGetResponse(generated.ProcessRequestMessage_Internal(createRequest(ERDM_Command.GET_COMMAND)), 0);
        #endregion
    }

    [Test, DependsOnTest(nameof(TestGetSetBURN_IN))]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<BurnInModule>().Single();
        generatedModule.HoursRemaining = 5;

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);

        var module = mockDevice.Modules.OfType<BurnInModule>().Single();
        Assert.That(module, Is.Not.Null);
        await Wait.UntilAsync(() => Wait.Holds(module.HoursRemaining, Is.EqualTo(5)));
        Assert.That(module.HoursRemaining, Is.EqualTo(5));

        await module.StartBurnIn(48);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.HoursRemaining, Is.EqualTo(48))
            && Wait.Holds(module.HoursRemaining, Is.EqualTo(48)));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.HoursRemaining, Is.EqualTo(48));
            Assert.That(module.HoursRemaining, Is.EqualTo(48));
        }

        await module.AbortBurnIn();
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.HoursRemaining, Is.Zero)
            && Wait.Holds(module.HoursRemaining, Is.Zero));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.HoursRemaining, Is.Zero);
            Assert.That(module.HoursRemaining, Is.Zero);
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
    private static RDMMessage createSetRequest(byte hours)
    {
        var request = createRequest(ERDM_Command.SET_COMMAND);
        request.ParameterData = new byte[] { hours };
        return request;
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
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Is.EqualTo(new byte[] { expected }));
        }
    }
    private static void assertSetAck(RDMMessage? response)
    {
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.SET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.Parameter, Is.EqualTo(MODULE_PARAMETER));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(0));
        }
    }

    class BurnInMockDevice : MockGeneratedDevice1
    {
        public BurnInMockDevice(UID uid) : base(uid, new IModule[] { new BurnInModule() })
        {
        }
    }
}
