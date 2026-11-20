using RDMSharp.Metadata;
using RDMSharp.PayloadObject;
using RDMSharp.RDM.Device.Module;
using RDMSharpTests.Devices.Mock;

namespace RDMSharpTests.RDM.Devices.Modules;

public class TestLockModule
{
    private LockModuleMockDevice? generated;

    private static readonly UID CONTROLLER_UID = new UID(0x18f, 20933);
    private static readonly UID DEVCIE_UID = new UID(211, 51500598);

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await MetadataFactory.AwaitInitialize();
    }

    [SetUp]
    public async Task Setup()
    {
        var defines = MetadataFactory.GetMetadataDefineVersions();
        Assert.That(defines, Is.Not.Null);
        generated = new LockModuleMockDevice(DEVCIE_UID);
        while (!generated.IsInitialized)
            await Task.Delay(100);
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
        var lockModule = generated.Modules.OfType<LockModule>().Single();
        Assert.That(lockModule, Is.Not.Null);
        Assert.That(lockModule.DisplayName, Is.EqualTo("Lock"));
    }

    [Test, DependsOnTest(nameof(TestDisplayName))]
    public async Task TestGetLOCK_STATE()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.LOCK_STATE), Is.True);
        await Wait.UntilAsync(() => generated.Modules.OfType<LockModule>().FirstOrDefault()?.LockState is not null);
        var lockStateModule = generated.Modules.OfType<LockModule>().FirstOrDefault();
        Assert.That(lockStateModule, Is.Not.Null);
        Assert.That(lockStateModule.LockState, Is.Not.Null);
        Assert.That(lockStateModule.LockState.Value, Is.Zero);
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LOCK_STATE,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LOCK_STATE));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.TypeOf(typeof(GetLockStateResponse)));
        }
        GetLockStateResponse lockStateResponse = (GetLockStateResponse)response.Value;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(lockStateResponse.CurrentLockStateId, Is.Zero);
            Assert.That(lockStateResponse.Count, Is.EqualTo(1));
        }

        #endregion

    }

    [Test, DependsOnTest(nameof(TestGetLOCK_STATE))]
    public async Task TestGetLOCK_PIN()
    {
        #region Test Basic
        Assert.That(generated, Is.Not.Null);
        Assert.That(generated.Parameters.Contains(ERDM_Parameter.LOCK_PIN), Is.True);
        await Wait.UntilAsync(() => generated.Modules.OfType<LockModule>().FirstOrDefault()?.LockPin is not null);
        var lockStateModule = generated.Modules.OfType<LockModule>().FirstOrDefault();
        Assert.That(lockStateModule, Is.Not.Null);
        Assert.That(lockStateModule.LockPin, Is.Not.Null);
        Assert.That(lockStateModule.LockPin.Value, Is.EqualTo(1234));
        RDMMessage request = new RDMMessage()
        {
            Command = ERDM_Command.GET_COMMAND,
            DestUID = DEVCIE_UID,
            SourceUID = CONTROLLER_UID,
            Parameter = ERDM_Parameter.LOCK_PIN,
            SubDevice = SubDevice.Root,
        };

        RDMMessage? response = generated.ProcessRequestMessage_Internal(request);
        Assert.That(response, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Command, Is.EqualTo(ERDM_Command.GET_COMMAND | ERDM_Command.RESPONSE));
            Assert.That(response.DestUID, Is.EqualTo(CONTROLLER_UID));
            Assert.That(response.SourceUID, Is.EqualTo(DEVCIE_UID));
            Assert.That(response.Parameter, Is.EqualTo(ERDM_Parameter.LOCK_PIN));
            Assert.That(response.SubDevice, Is.EqualTo(SubDevice.Root));
            Assert.That(response.ResponseType, Is.EqualTo(ERDM_ResponseType.ACK));
            Assert.That(response.ParameterData, Has.Length.EqualTo(2));
            Assert.That(response.Value, Is.TypeOf(typeof(ushort)));
        }
        ushort lockPin = (ushort)response.Value;
        Assert.That(lockPin, Is.EqualTo(1234));

        #endregion

    }

    [Test, DependsOnTest(nameof(TestGetLOCK_PIN)), MaxTime(15000)]
    public async Task TestRemoteDevice()
    {
        Assert.That(generated, Is.Not.Null);
        var generatedModule = generated.Modules.OfType<LockModule>().Single();
        Assert.That(generatedModule, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.LockState, Is.Zero);
            Assert.That(generatedModule.LockPin, Is.EqualTo(1234));
        }

        using MockDevice mockDevice = new MockDevice(DEVCIE_UID);
        while (!mockDevice.IsInitialized)
            await Task.Delay(100);
        while (!mockDevice.AllDataPulled)
            await Task.Delay(100);

        await Wait.UntilAsync(() =>
            Wait.Holds(mockDevice.Modules.OfType<LockModule>().Single().LockState, Is.Zero)
            && Wait.Holds(mockDevice.Modules.OfType<LockModule>().Single().LockPin, Is.EqualTo(1234)));

        var module = mockDevice.Modules.OfType<LockModule>().Single();
        Assert.That(module, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(module.LockState, Is.Zero);
            Assert.That(module.LockPin, Is.EqualTo(1234));
        }
        await module.SetLockState(1);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.LockState, Is.EqualTo(1))
            && Wait.Holds(module.LockState, Is.EqualTo(1))
            && Wait.Holds(generatedModule.LockPin, Is.EqualTo(1234))
            && Wait.Holds(module.LockPin, Is.EqualTo(1234)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.LockState, Is.EqualTo(1));
            Assert.That(module.LockState, Is.EqualTo(1));
            Assert.That(generatedModule.LockPin, Is.EqualTo(1234));
            Assert.That(module.LockPin, Is.EqualTo(1234));
        }

        await module.SetLockPin(4321);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.LockState, Is.EqualTo(1))
            && Wait.Holds(module.LockState, Is.EqualTo(1))
            && Wait.Holds(generatedModule.LockPin, Is.EqualTo(4321))
            && Wait.Holds(module.LockPin, Is.EqualTo(4321)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.LockState, Is.EqualTo(1));
            Assert.That(module.LockState, Is.EqualTo(1));
            Assert.That(generatedModule.LockPin, Is.EqualTo(4321));
            Assert.That(module.LockPin, Is.EqualTo(4321));
        }

        await module.SetLockState(0);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.LockState, Is.Zero)
            && Wait.Holds(module.LockState, Is.Zero)
            && Wait.Holds(generatedModule.LockPin, Is.EqualTo(4321))
            && Wait.Holds(module.LockPin, Is.EqualTo(4321)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.LockState, Is.Zero);
            Assert.That(module.LockState, Is.Zero);
            Assert.That(generatedModule.LockPin, Is.EqualTo(4321));
            Assert.That(module.LockPin, Is.EqualTo(4321));
        }

        await module.SetLockPin(1234);
        await Wait.UntilAsync(() =>
            Wait.Holds(generatedModule.LockState, Is.Zero)
            && Wait.Holds(module.LockState, Is.Zero)
            && Wait.Holds(generatedModule.LockPin, Is.EqualTo(1234))
            && Wait.Holds(module.LockPin, Is.EqualTo(1234)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedModule.LockState, Is.Zero);
            Assert.That(module.LockState, Is.Zero);
            Assert.That(generatedModule.LockPin, Is.EqualTo(1234));
            Assert.That(module.LockPin, Is.EqualTo(1234));
        }
    }

    class LockModuleMockDevice : MockGeneratedDevice1
    {
        public LockModuleMockDevice(UID uid) : base(uid, new IModule[] { new LockModule(0, 1234, new RDMLockStateDescription(1, "Lock")) })
        {
        }
    }
}